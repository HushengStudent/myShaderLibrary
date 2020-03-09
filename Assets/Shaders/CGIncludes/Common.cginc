#ifndef MY_SHADER_LIBRARY_COMMON
#define MY_SHADER_LIBRARY_COMMON

#include "UnityCG.cginc"

#define ftime (_Time.y)

#define _LightDir (normalize(_WorldSpaceLightPos0.xyz))	

fixed rand(fixed2 seed) {
	return frac(sin(dot(seed, fixed2(12.9898, 78.233))) * 43758.5453);
}

#endif