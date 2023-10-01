#ifndef WIWIW_GET_CLOUDS_NOISE_INCLUDED
#define WIWIW_GET_CLOUDS_NOISE_INCLUDED

// --------------------------------------------------------

#include "../../../Utilities/Shaders/Library/GetSmoothRemap.hlsl"
#include "GetCloudsGradient.hlsl"
#include "GetPerlinNoise.hlsl"

// --------------------------------------------------------

// inPosition: gradient position in range [-1, 1]
// inDensityParams.x: scale of clouds noise in any range
// inDensityParams.y: contrast of clouds noise in range [0, 1]
// inDensityParams.z: density of clouds noise in range [0, 1]
// inGradientParams.x: clouds fade in start pos in range [0, 1]
// inGradientParams.y: clouds fade in final pos in range [0, 1]
// inGradientParams.z: clouds fade out start pos in range [0, 1]
// inGradientParams.w: clouds fade out final pos in range [0, 1]
float Wiwiw_GetCloudsNoise(in float3 inPosition, in float3 inDensityParams, in float4 inGradientParams)
{
	float cloudsNoise = Wiwiw_GetPerlinNoise(inPosition * inDensityParams.x);
	cloudsNoise = Wiwiw_GetSmoothRemap(cloudsNoise, lerp(0.0, 1.0 - inDensityParams.z, inDensityParams.y), lerp(1.0, 1.0 - inDensityParams.z, inDensityParams.y), 0.0, 1.0);
	cloudsNoise = cloudsNoise * Wiwiw_GetCloudsGradient(inPosition.y, inGradientParams);

	return cloudsNoise;
}

void Wiwiw_GetCloudsNoise_float(in float3 inPosition, in float3 inDensityParams, in float4 inGradientParams, out float outValue)
{
	outValue = Wiwiw_GetCloudsNoise(inPosition, inDensityParams, inGradientParams);
}

// --------------------------------------------------------

#endif // WIWIW_GET_CLOUDS_NOISE_INCLUDED
