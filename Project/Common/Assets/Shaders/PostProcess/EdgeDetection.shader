﻿Shader  "myShaderLibrary/PostProcess/EdgeDetection"
{
	Properties
	{
		_EdgeOnly ("Edge Only", Range(0,1)) = 0
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
	}

    CGINCLUDE
        #include "UnityCG.cginc"
        #include "Assets/Shaders/CGIncludes/Common.cginc"
			
		sampler2D _MainTex;  
		uniform half4 _MainTex_TexelSize;
		fixed _EdgeOnly;
		fixed4 _EdgeColor;
		fixed4 _BackgroundColor;
			
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

		struct v2f
		{
			float4 vertex : SV_POSITION;
			half2 uv[9] : TEXCOORD0;
		};
			  
		v2f vert(appdata v)
		{
			v2f o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            half2 uv = TransformTriangleVertexToUV(v.vertex.xy);
            uv = UVStartAtTop(uv);

			o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
			o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
			o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
			o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
			o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
			o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
			o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
			o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
			o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);

			return o;
		}
			
		fixed luminance(fixed4 color)
		{
			return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
		}
			
		half Sobel(v2f i)
		{
			const half Gx[9] = {-1,  0,  1,
									-2,  0,  2,
									-1,  0,  1};
			const half Gy[9] = {-1, -2, -1,
									0,  0,  0,
									1,  2,  1};		
			half texColor;
			half edgeX = 0;
			half edgeY = 0;
			for (int it = 0; it < 9; it++)
			{
				texColor = luminance(tex2D(_MainTex, i.uv[it]));
				edgeX += texColor * Gx[it];
				edgeY += texColor * Gy[it];
			}
			half edge = 1 - abs(edgeX) - abs(edgeY);
			return edge;
		}
			
		fixed4 fragSobel(v2f i) : SV_Target
		{
			half edge = Sobel(i);
			fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[4]), edge);
			fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
			return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
 		}
    ENDCG

	SubShader
	{
		Pass
		{  
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment fragSobel
			ENDCG
		} 
	}

	FallBack Off
}