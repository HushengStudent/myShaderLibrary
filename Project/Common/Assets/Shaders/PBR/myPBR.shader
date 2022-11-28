//此测试shader来自雨轩，假如需要分享请备注雨轩的知乎主页链接https://www.zhihu.com/people/yu-xuan-68-77
Shader "yuxuan/PBR"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" { }
		_MetallicTex("Metallic(R),Smoothness(A)", 2D) = "white" { }
		_Metallic("Metallic", Range(0, 1)) = 1.0
		_Glossiness("Smoothness", Range(0, 1)) = 1.0
		[Normal]_Normal("NormalMap", 2D) = "bump" { }
		_OcclussionTex("Occlusion", 2D) = "white" { }
		_AO("AO", Range(0, 1)) = 1.0
		_Emission("Emission", Color) = (0, 0, 0, 1)
	}
	
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			//因为需要灯光设定和Unity配合，所以要加上前向渲染标签。
			Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM

			//顶点片段着色器
			#pragma vertex vert
			#pragma fragment frag
			//指定平台，也可以省略
			#pragma target 3.0
			//雾效和灯光的关键字
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			//一些会用到的cginc文件
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "AutoLight.cginc"
			#include "Assets/Shaders/CGIncludes/myPBR.cginc"
			//...

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MetallicTex;
			fixed _Metallic;
			fixed _Glossiness;
			sampler2D _OcclussionTex;
			fixed _AO;
			half3 _Emission;
			sampler2D _Normal;
			//...

			struct v2f
			{
				float4 pos: SV_POSITION;//裁剪空间位置输出
				float2 uv: TEXCOORD0; // 贴图UV
				float3 worldPos: TEXCOORD1;//世界坐标
				float3 tSpace0: TEXCOORD2;//TNB矩阵0
				float3 tSpace1: TEXCOORD3;//TNB矩阵1
				float3 tSpace2: TEXCOORD4;//TNB矩阵2

				UNITY_FOG_COORDS(5)//雾效坐标
				UNITY_SHADOW_COORDS(6)//阴影坐标 _ShadowCoord

				//如果需要采样球谐函数，则输入球谐函数参数。
				#if UNITY_SHOULD_SAMPLE_SH
					half3 sh: TEXCOORD7; // SH
				#endif
			};

			// vertex shader
			//这里没有写appdata结构体，直接采用内置的appdata_Full
			v2f vert(appdata_full v)
			{
				v2f o;//定义返回v2f 结构体o
				UNITY_INITIALIZE_OUTPUT(v2f, o);//将o初始化。
				o.pos = UnityObjectToClipPos(v.vertex);//计算齐次裁剪空间下的坐标位置
				//这里的uv只定义了两个分量。TranformTex方法加入了贴图的TillingOffset值。
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;//世界空间坐标计算。
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);//世界空间法线计算
				half3 worldTangent = UnityObjectToWorldDir(v.tangent);//世界空间切线计算
				//利用切线和法线的叉积来获得副切线，tangent.w分量确定副切线方向正负，unity_WorldTransformParams.w判定模型是否有变形翻转。
				half3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w * unity_WorldTransformParams.w;

				//组合TBN矩阵，用于后续的切线空间法线计算。
				o.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
				o.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
				o.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);

				// SH/ambient和顶点光照写入o.sh里
				#ifndef LIGHTMAP_ON
					#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
						o.sh = 0;
						// Approximated illumination from non-important point lights
						//如果有顶点光照的情况（超出系统限定的灯光数或者被设置为non-important灯光）
						#ifdef VERTEXLIGHT_ON
							o.sh += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,unity_4LightAtten0, o.worldPos, worldNormal);
						#endif
						//球谐光照计算（光照探针，超过顶点光照数量的球谐灯光）
						o.sh = ShadeSHPerVertex(worldNormal, o.sh);
					#endif
				#endif // !LIGHTMAP_ON

				UNITY_TRANSFER_LIGHTING(o, v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
				//在appdata_full结构体里。v.texcoord1就是第二套UV，也就是光照贴图的UV。
				//计算并传递阴影坐标

				UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader。计算传递雾效的坐标。

				return o;
			}

			// fragment shader
			fixed4 frag(v2f i) : SV_Target
			{
				half3 normalTex = UnpackNormal(tex2D(_Normal, i.uv));//使用法线的采样方式对法线贴图进行采样。
				//切线空间法线（带贴图）转向世界空间法线。
				half3 worldNormal = half3(dot(i.tSpace0, normalTex), dot(i.tSpace1, normalTex), dot(i.tSpace2, normalTex));
				worldNormal = normalize(worldNormal);//所有传入的“向量”最好归一化一下
				//计算灯光方向：注意这个方法已经包含了对灯光的判定。
				//其实在forwardbase pass中，可以直接用灯光坐标代替这个方法，因为只会计算Directional Light。
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));//片段指向摄像机方向viewDir

				SurfaceOutputStandard o;//声明变量
				UNITY_INITIALIZE_OUTPUT(SurfaceOutputStandard, o);//初始化里面的信息。避免有的时候报错干扰
				fixed4 AlbedoColorSampler = tex2D(_MainTex, i.uv) * _Color;//采样颜色贴图，同时乘以控制的TintColor
				o.Albedo = AlbedoColorSampler.rgb;//颜色分量，a分量在后面
				o.Emission = _Emission;//自发光
				fixed4 MetallicSmoothnessSampler = tex2D(_MetallicTex, i.uv);//采样Metallic-Smoothness贴图
				o.Metallic = MetallicSmoothnessSampler.r * _Metallic;//r通道乘以控制色并赋予金属度
				o.Smoothness = MetallicSmoothnessSampler.a * _Glossiness;//a通道乘以控制色并赋予光滑度
				o.Alpha = AlbedoColorSampler.a;//单独赋予透明度
				o.Occlusion = tex2D(_OcclussionTex, i.uv) * _AO; //赋予AO
				o.Normal = worldNormal;//赋予法线

				// compute lighting & shadowing factor
				//计算光照衰减和阴影
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos)

				//初始化全局光照，输入直射光参数。间接光参数置零待更新。
				// Setup lighting environment
				UnityGI gi;//声明变量
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);//初始化归零
				gi.indirect.diffuse = 0;//indirect部分先给0参数，后面需要计算出来。这里只是示意
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;//unity内置的灯光颜色变量
				gi.light.dir = lightDir;//赋予之前计算的灯光方向。

				//初始化giInput并赋予已有的值。此参数为gi计算所需要的输入参数。
				// Call GI (lightmaps/SH/reflections) lighting function
				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = i.worldPos;
				giInput.worldViewDir = worldViewDir;
				giInput.atten = atten;//在之前的光照衰减里面已经被计算。其中包含阴影的计算了。

				//球谐光照和环境光照输入（已在顶点着色器里计算，这里只是输入）
				#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					giInput.ambient = i.sh;
				#else
					giInput.ambient.rgb = 0.0;
				#endif

				//反射探针相关
				giInput.probeHDR[0] = unity_SpecCube0_HDR;
				giInput.probeHDR[1] = unity_SpecCube1_HDR;
				#if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
					giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
				#endif

				#ifdef UNITY_SPECCUBE_BOX_PROJECTION
					giInput.boxMax[0] = unity_SpecCube0_BoxMax;
					giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
					giInput.boxMax[1] = unity_SpecCube1_BoxMax;
					giInput.boxMin[1] = unity_SpecCube1_BoxMin;
					giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
				#endif

				//基于PBS的全局光照（gi变量）的计算函数。计算结果是gi的参数（Light参数和Indirect参数）。注意这一步还没有做真的光照计算。
				LightingStandard_GI(o, giInput, gi);
				fixed4 c = 0;
				// realtime lighting: call lighting function
				//PBS计算
				c += LightingStandard1(o, worldViewDir, gi);

				//叠加雾效。
				UNITY_EXTRACT_FOG(i);//此方法定义了一个片段着色器里的雾效坐标变量，并赋予传入的雾效坐标。
				UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
				// return unity_IndirectSpecColor;
				return c;
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
