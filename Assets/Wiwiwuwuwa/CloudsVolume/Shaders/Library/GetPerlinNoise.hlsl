#ifndef WIWIW_GET_PERLIN_NOISE_INCLUDED
#define WIWIW_GET_PERLIN_NOISE_INCLUDED

// --------------------------------------------------------

#include "GetPerlinHash.hlsl"

// --------------------------------------------------------

float Wiwiw_GetPerlinNoise(in float3 inValue)
{
    const float3 integerCoords = floor(inValue);
    const float3 fractionalCoords = frac(inValue);
    const float3 fadeCurve = smoothstep(0.0, 1.0, fractionalCoords);
    const float1 perlinNoise = lerp
    (
        lerp
        (
            lerp
            (
                dot(Wiwiw_GetPerlinHash(integerCoords), fractionalCoords),
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(1.0, 0.0, 0.0)), fractionalCoords - float3(1.0, 0.0, 0.0)),
                fadeCurve.x
            ),
            lerp
            (
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(0.0, 1.0, 0.0)), fractionalCoords - float3(0.0, 1.0, 0.0)),
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(1.0, 1.0, 0.0)), fractionalCoords - float3(1.0, 1.0, 0.0)),
                fadeCurve.x
            ),
            fadeCurve.y
        ),
        lerp
        (
            lerp
            (
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(0.0, 0.0, 1.0)), fractionalCoords - float3(0.0, 0.0, 1.0)),
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(1.0, 0.0, 1.0)), fractionalCoords - float3(1.0, 0.0, 1.0)),
                fadeCurve.x
            ),
            lerp
            (
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(0.0, 1.0, 1.0)), fractionalCoords - float3(0.0, 1.0, 1.0)),
                dot(Wiwiw_GetPerlinHash(integerCoords + float3(1.0, 1.0, 1.0)), fractionalCoords - float3(1.0, 1.0, 1.0)),
                fadeCurve.x
            ),
            fadeCurve.y
        ),
        fadeCurve.z
    );

    return smoothstep(-1.0, 1.0, perlinNoise.x);
}

void Wiwiw_GetPerlinNoise_float(in float3 inValue, out float outValue)
{
	outValue = Wiwiw_GetPerlinNoise(inValue);
}

// --------------------------------------------------------

#endif // WIWIW_GET_PERLIN_NOISE_INCLUDED
