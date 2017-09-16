Shader "myShaderLibrary/004DiffuseVertex" {
	Properties{
		_Diffuse("Diffuse Color",Color) = (1,1,1,1)//材质漫反射颜色
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
				fixed3 color : COLOR;
			};
				
			v2f vert(a2v v) { 
				v2f f;
				f.position = mul(UNITY_MATRIX_MVP,v.vertex);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				//nvidia：
				//Returns the normalized version of a vector, meaning a vector 
				//in the same direction as the original vector 
				//but with a Euclidean length of one
				//法线的变换与顶点的变换是有区别的（非等比例缩放）
				fixed3 normalDir = normalize( mul( v.normal, (float3x3) _World2Object) );
				//_WorldSpaceLightPos0是Unity为我们赋值的一个表示场景中第一个平行光的位置
				fixed3 lightDir =  normalize( _WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir), 0) *_Diffuse.rgb;  
				f.color = diffuse + ambient;
				return f;
			} 

			fixed4 frag(v2f f) :SV_Target//SV_Target (render target pixel color) 
			{
				return fixed4(f.color,1);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
