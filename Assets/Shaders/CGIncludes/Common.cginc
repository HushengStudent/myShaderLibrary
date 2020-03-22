#ifndef MY_SHADER_LIBRARY_COMMON
#define MY_SHADER_LIBRARY_COMMON

#include "UnityCG.cginc"

#define ftime (_Time.y)

#define _LightDir (normalize(_WorldSpaceLightPos0.xyz))	

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