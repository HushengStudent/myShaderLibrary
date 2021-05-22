#ifndef MY_SHADER_LIBRARY_COMMON
#define MY_SHADER_LIBRARY_COMMON

#include "UnityCG.cginc"

fixed4 AddMask(fixed4 color,fixed4 maskColor) 
{
    color *= step(0.01, 1-maskColor.a);  
    return color + maskColor;  
}

float2 TransformTriangleVertexToUV(float2 vertex)
{
    float2 uv = (vertex + 1.0) * 0.5;
    return uv;
}

float2 UVStartAtTop(float2 uv)
{
    #if UNITY_UV_STARTS_AT_TOP
    uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
    #endif
    return uv;
}

fixed rand(fixed2 seed) {
	return frac(sin(dot(seed, fixed2(12.9898, 78.233))) * 43758.5453);
}

fixed4 Blur(fixed2 uv, sampler2D source, fixed Intensity)
{
	fixed distance = 0.00390625f * Intensity;
	fixed4 result = fixed4 (0, 0, 0, 0);

	fixed2 texCoord = fixed2(0, 0);
	texCoord = uv + fixed2(-distance, -distance);
	result += tex2D(source, texCoord);

	texCoord = uv + fixed2(-distance, 0);
	result += 2.0 * tex2D(source, texCoord);

	texCoord = uv + fixed2(-distance, distance);
	result += tex2D(source, texCoord);

	texCoord = uv + fixed2(0, -distance);
	result += 2.0 * tex2D(source, texCoord);

	texCoord = uv;
	result += 4.0 * tex2D(source, texCoord);

	texCoord = uv + fixed2(0, distance);
	result += 2.0 * tex2D(source, texCoord);

	texCoord = uv + fixed2(distance, -distance);
	result += tex2D(source, texCoord);

	texCoord = uv + fixed2(distance, 0);
	result += 2.0* tex2D(source, texCoord);

	texCoord = uv + fixed2(distance, -distance);
	result += tex2D(source, texCoord);

	result = result * 0.0625;
	return result;
}

#endif