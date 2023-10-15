#ifndef WIWIW_GET_PERLIN_HASH_INCLUDED
#define WIWIW_GET_PERLIN_HASH_INCLUDED

// --------------------------------------------------------

float3 Wiwiw_GetPerlinHash(in float3 inValue)
{
    const float3 hashCoefficients = float3
    (
        dot(inValue, float3(127.1, 311.7, 74.7)),
        dot(inValue, float3(269.5, 183.3, 246.1)),
        dot(inValue, float3(113.5, 271.9, 124.6))
	);

    return mad(frac(sin(hashCoefficients) * 43758.5453123), 2.0, -1.0);
}

void Wiwiw_GetPerlinHash_float(in float3 inValue, out float3 outValue)
{
    outValue = Wiwiw_GetPerlinHash(inValue);
}

// --------------------------------------------------------

#endif // WIWIW_GET_PERLIN_HASH_INCLUDED
