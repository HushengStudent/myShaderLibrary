// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//顶点动画
Shader "myShaderLibrary/Common/14VertexAnima" 
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Color("Color Tint", Color) = (1, 1, 1, 1)
 		_WaveLength("Wave Length", Float) = 0.5
 		_Speed("Speed", Float) = 0.5
	}

	SubShader
	{
		//Need to disable batching because of the vertex animation
		Tags {"DisableBatching"="True"}
		Pass 
		{
			Tags { "LightMode"="ForwardBase" }
			Cull Off
			CGPROGRAM  
			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc" 
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _WaveLength;
			float _Speed;
			
			struct a2v
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};
			
			v2f vert(a2v v)
			{
				v2f o;
				float4 offset;
				offset.yzw = float3(0.0, 0.0, 0.0);
				offset.x = sin(_Time.y + v.vertex.x * _WaveLength + v.vertex.y * _WaveLength + v.vertex.z * _WaveLength);
				o.pos = UnityObjectToClipPos(v.vertex + offset);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv +=  float2(0.0, _Time.y * _Speed);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb *= _Color.rgb;
				return c;
			}
			ENDCG
		}
	}
    FallBack "Diffuse"
}
