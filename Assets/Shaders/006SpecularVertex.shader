Shader "myShaderLibrary/006SpecularVertex" {
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)
		_Specular("Specular Color",Color)=(1,1,1,1)
		_Gloss("Gloss",Range(8,200)) = 10

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
			fixed4 _Specular;
			half _Gloss;
				
			//application to vertex
			struct a2v {
			float4 vertex:POSITION;
			float3 normal:NORMAL;

			};

			struct v2f {
				float4 position:SV_POSITION;
				fixed3 color : COLOR;
			}; 
				
			v2f vert(a2v v) { 
				v2f f;
				f.position = mul(UNITY_MATRIX_MVP,v.vertex);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 normalDir = normalize( mul( v.normal, (float3x3) _World2Object) );
				fixed3 lightDir =  normalize( _WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) *_Diffuse.rgb; 
				fixed3 reflectDir = normalize(reflect(-lightDir, normalDir));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(v.vertex, _World2Object).xyz);
				fixed3 specular = _LightColor0.rgb *  _Specular.rgb * pow(max(dot(reflectDir, viewDir), 0), _Gloss);
				f.color = diffuse + ambient + specular;
				return f;
			} 

			fixed4 frag(v2f f) :SV_Target{
				return fixed4(f.color,1);
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}
