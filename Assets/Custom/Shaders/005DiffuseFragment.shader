Shader "myShaderLibrary/005DiffuseFragment" {
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)
	}
		SubShader{
			Pass{

			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag
				
			fixed4 _Diffuse;
			
			//application to vertex
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct v2f {
				float4 position:SV_POSITION;
				fixed3 worldNormalDir : TEXCOORD0;
			};
				
			v2f vert(a2v v) { 
				v2f f;
				f.position = mul(UNITY_MATRIX_MVP,v.vertex);
				f.worldNormalDir = mul(v.normal, (float3x3) _World2Object);
				return f;
			}

			fixed4 frag(v2f f) :SV_Target{
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 normalDir = normalize(f.worldNormalDir);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) *_Diffuse.rgb; 
				fixed3 tempColor = diffuse + ambient;
				return fixed4(tempColor,1);
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}
