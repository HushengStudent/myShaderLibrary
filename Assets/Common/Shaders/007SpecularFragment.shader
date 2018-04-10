// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "myShaderLibrary/Common/007SpecularFragment" {
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
				float3 worldNormal : TEXCOORD0;
				float3 worldVertex :TEXCOORD1;
			}; 
				
			v2f vert(a2v v) { 
				v2f f;
				f.position = mul(UNITY_MATRIX_MVP,v.vertex);
				f.worldNormal = mul(v.normal, (float3x3) unity_WorldToObject);
				f.worldVertex = mul(unity_WorldToObject,v.vertex).xyz;
				return f;
			} 

			fixed4 frag(v2f f) :SV_Target{
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 normalDir = normalize(f.worldNormal);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) *_Diffuse.rgb;
				fixed3 reflectDir = normalize(reflect(-lightDir, normalDir));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - f.worldVertex );
				fixed3 specular = _LightColor0.rgb *  _Specular.rgb * pow(max(dot(reflectDir, viewDir), 0), _Gloss);
				fixed3 tempColor = diffuse + ambient + specular;
				return fixed4(tempColor,1);
			}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
