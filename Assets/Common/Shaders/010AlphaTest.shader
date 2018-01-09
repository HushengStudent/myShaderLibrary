Shader "myShaderLibrary/010AlphaTest" {
	Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap",2D) = "Bump"{}
        _SpecColor("SpecularColor",Color) = (1,1,1,1)
        _Shininess("Shininess",Float) = 10

		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader 
    {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
            uniform sampler2D _NormalMap;
            uniform float4 _SpecColor;
			//控制凸起程度
            uniform float _Shininess;
            uniform float4 _LightColor0;

			fixed _Cutoff;

            struct VertexOutput 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float3 lightDir:TEXCOORD1;
                float3 viewDir:TEXCOORD2;
            };

            VertexOutput vert(appdata_tan v)
            {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                o.uv = v.texcoord.xy;
				//得到Object2TangentMatrix矩阵
				//这四行代码可以使用Unity定义的一个宏TANGENT_SPACE_ROTATION来代替
                float3 normal = v.normal;
                float3 tangent = v.tangent;
                float3 binormal= cross(v.normal,v.tangent.xyz) * v.tangent.w;//tangent.w决定第三个坐标轴，值为正负1
                float3x3 Object2TangentMatrix = float3x3(tangent,binormal,normal);
				//之所以变换到切线空间，是为了减少片段着色器的计算量
                o.lightDir = mul(Object2TangentMatrix,ObjSpaceLightDir(v.vertex));
                o.viewDir = mul(Object2TangentMatrix,ObjSpaceViewDir(v.vertex));
                return o;
            }

            float4 frag(VertexOutput input):COLOR
            {
                float3 lightDir = normalize(input.lightDir);
                float3 viewDir = normalize(input.viewDir);
                float4 encodedNormal = tex2D(_NormalMap,input.uv);
				//法线[-1,1],颜色[0,1]
                float3 normal = float3(2.0*encodedNormal.ag - 1,0.0);
				//保证normal为正
                normal.z = sqrt(1 - dot(normal,normal));
                float4 texColor = tex2D(_MainTex,input.uv);
				
				clip (texColor.a -_Cutoff*0.2);

                float3 ambient = texColor.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;
                float3 diffuseReflection = texColor.rgb * _LightColor0.rgb * max(0,dot(normal,lightDir));
                float facing;
                if(dot(normal,lightDir)<0)
                {
                    facing = 0;
                }
                else
                {
                    facing = 1;
                }
                float3 specularRelection = 
				_SpecColor.rgb * _LightColor0.rgb * facing * pow(max(0,dot(reflect(-lightDir,normal),viewDir)),_Shininess);
                return float4(ambient + diffuseReflection + specularRelection,1);
            }
            ENDCG
        }
    } 
    FallBack "Diffuse"
}
