Shader "myShaderLibrary/008SingleTexture" {
	Properties{
		_Color("Color",Color)=(1,1,1,1)
		_Specular("Specular Color",Color)=(1,1,1,1)
		_Gloss("Gloss",Range(8,200)) = 10
		_MainTex("Main Tex",2D) = "white"{}//声明纹理代替谩发射颜色(全白纹理)
	}
		SubShader{
			Pass{

			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Specular;
			half _Gloss;
			sampler2D _MainTex;
			float4 _MainTex_ST;//纹理类型的属性声明(纹理名_ST)
			fixed4 _Color;

			struct a2v 
			{
			float4 vertex:POSITION;
			float3 normal:NORMAL;
			float4 texcoord:TEXCOORD0;
			};

			struct v2f {
				float4 position:SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float4 worldVertex :TEXCOORD1;
				float2 uv:TEXCOORD2;
				
			}; 
				
			v2f vert(a2v v) 
			{ 
				v2f f;
				f.position = mul(UNITY_MATRIX_MVP,v.vertex);
				f.worldNormal = mul(v.normal, (float3x3) _World2Object);
				f.worldVertex = mul(v.vertex, _World2Object);
				//对顶点纹理坐标进行变换，得到最终的纹理坐标
				f.uv = v.texcoord.xy * _MainTex_ST.xy  + _MainTex_ST.zw;
				return f;
			} 

			fixed4 frag(v2f f) :SV_Target
			{
				//材质的反射率 = tex2D纹理采样函数
				fixed3 texColor = tex2D(_MainTex, f.uv.xy)*_Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 normalDir = normalize(f.worldNormal);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) *texColor.rgb;
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - f.worldVertex );
				fixed3 halfDir = normalize(viewDir + lightDir);
				fixed3 specular = _LightColor0.rgb *  _Specular.rgb * pow(max(dot(normalDir, halfDir), 0), _Gloss);
				fixed3 tempColor = diffuse + ambient*texColor + specular;
				return fixed4(tempColor,1);
			}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
