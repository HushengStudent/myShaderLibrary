Shader "myShaderLibrary/UI/MergeAlphaChannel"
{
	Properties
	{
		_MainTex("RGB Texture", 2D) = "black" {}
		_AlphaTex("Alpha Texture",2D) = "white" {}
	}
	SubShader
	{
		LOD 200
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			//Offset - 1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 mainTex = tex2D(_MainTex, IN.texcoord);
				fixed4 alphaTex = tex2D(_AlphaTex, IN.texcoord);
				mainTex.a = alphaTex.r;
				return mainTex * IN.color;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			//Offset - 1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
