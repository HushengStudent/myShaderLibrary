// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//序列帧动画
Shader "myShaderLibrary/Common/013FrameAnima" 
{
	Properties
	{
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Sequence Image", 2D) = "white" {}
    	_HorizontalAmount("Horizontal Amount", Float) = 8
    	_VerticalAmount("Vertical Amount", Float) = 8
    	_Speed("Speed", Range(1, 100)) = 40
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;//纹理
			float _HorizontalAmount;
			float _VerticalAmount;
			float _Speed;
			  
			struct a2v
			{  
			    float4 vertex : POSITION; 
			    float2 texcoord : TEXCOORD0;
			};  
			
			struct v2f
			{  
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};  
			
			v2f vert (a2v v) 
			{  
				v2f o;  
				o.pos = UnityObjectToClipPos(v.vertex);  
				//TRANSFORM_TEX定义在UnityCG.cginc里:
				//Transforms 2D UV by scale/bias property
				//#define TRANSFORM_TEX(tex,name)(tex.xy*name##_ST.xy + name##_ST.zw)

				//TRANSFORM_TEX主要作用是拿顶点的uv去和材质球的tiling和offset作运算,确保材质球里的缩放和偏移设置是正确的
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  
				return o;
			}  
			
			fixed4 frag (v2f i) : SV_Target 
			{
				float time = floor(_Time.y * _Speed);  
				float row = floor(time / _HorizontalAmount);
				float column = time - row * _HorizontalAmount;
				
				half2 uv = i.uv + half2(column, -row);
				uv.x /=  _HorizontalAmount;
				uv.y /= _VerticalAmount;
				
				fixed4 c = tex2D(_MainTex, uv);
				c.rgb *= _Color;
				
				return c;
			}
			ENDCG
		}  
	}
    FallBack "Diffuse"
}
