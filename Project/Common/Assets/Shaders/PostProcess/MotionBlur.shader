Shader "myShaderLibrary/PostProcess/MotionBlur"
{
    Properties
	{
		_BlurAmount ("Blur Amount", Range(0,1)) = 0.2
	}

	CGINCLUDE
		#include "UnityCG.cginc"
        #include "Assets/Shaders/CGIncludes/Common.cginc"

        #pragma vertex vert

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            //UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
        };
		
		sampler2D _MainTex;
        float4 _MainTex_TexelSize;
		fixed _BlurAmount;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            o.uv = UVStartAtTop(o.uv);
            return o;
        }
		
		fixed4 fragRGB (v2f i) : SV_Target
		{
			return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
		}
		
		half4 fragA (v2f i) : SV_Target
		{
			return tex2D(_MainTex, i.uv);
		}
	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			CGPROGRAM
			#pragma fragment fragRGB  
			ENDCG
		}
		
		Pass
		{   
			Blend One Zero
			ColorMask A
			CGPROGRAM  
			#pragma fragment fragA
			ENDCG
		}
	}
 	FallBack Off
}