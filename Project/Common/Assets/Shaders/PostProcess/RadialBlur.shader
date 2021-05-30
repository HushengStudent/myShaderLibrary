Shader "myShaderLibrary/PostProcess/RadialBlur"
{
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
		
        uniform half4 _Params;
	
	    #define _RadialRadius _Params.x
	    #define _RadialIteration _Params.y
	    #define _RadialCenter _Params.zw

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            o.uv = UVStartAtTop(o.uv);
            return o;
        }
		
		half4 RadialBlur(v2f i)
	    {
		    float2 blurVector = (_RadialCenter - i.uv.xy) * _RadialRadius;
		
		    half4 acumulateColor = half4(0, 0, 0, 0);
		
		    [unroll(30)]
		    for (int j = 0; j < _RadialIteration; j ++)
		    {
			    acumulateColor += tex2D(_MainTex, i.uv);
			    i.uv.xy += blurVector;
		    }
		
		    return acumulateColor / _RadialIteration;
	    }
	
	    half4 Frag(v2f i): SV_Target
	    {
		    return RadialBlur(i);
	    }
	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off

		Pass
		{
			CGPROGRAM
            #pragma fragment Frag
            ENDCG
		}
	}
 	FallBack Off
}