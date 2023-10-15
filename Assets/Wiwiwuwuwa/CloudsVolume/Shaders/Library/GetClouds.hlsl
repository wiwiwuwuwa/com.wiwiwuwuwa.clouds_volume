#ifndef WIWIW_GET_CLOUDS_INCLUDED
#define WIWIW_GET_CLOUDS_INCLUDED

// --------------------------------------------------------

Texture3D<float> _Wiwiw_DensityTexture;

SamplerState sampler_Wiwiw_DensityTexture;

float4x4 _Wiwiw_DensityWorldToObjectMatrix;

// [0]: fade in start pos in meters
// [1]: fade in final pos in meters
// [2]: fade out start pos in meters
// [3]: fade out final pos in meters
float _Wiwiw_CloudsGradientParams[4];

// [0]: fade in start value in range [0, 1]
// [1]: fade in final value in range [0, 1]
// [2]: fade out start value in range [0, 1]
// [3]: fade out final value in range [0, 1]
float _Wiwiw_CloudsGradientValues[4];

float _Wiwiw_CloudsContrast;

float _Wiwiw_CloudsMidpoint;

// --------------------------------------------------------

float Wiwiw_GetCloudsBlend(in float3 inPosWS)
{
    float result = 0.0;

    for (float i = 0.0; i < 3.0; i++)
    {
        const float startPos = _Wiwiw_CloudsGradientParams[i];
        const float finalPos = _Wiwiw_CloudsGradientParams[i + 1.0];
        const float isInInterval = inPosWS.y >= startPos && inPosWS.y < finalPos;

        const float startVal = _Wiwiw_CloudsGradientValues[i];
        const float finalVal = _Wiwiw_CloudsGradientValues[i + 1.0];
        const float valueOfInterval = lerp(startVal, finalVal, smoothstep(startPos, finalPos, inPosWS.y));

        result += isInInterval * valueOfInterval;
    }

    return result;
}

float Wiwiw_GetCloudsValue(in float3 inPosWS)
{
    const float3 noisePos = mul(_Wiwiw_DensityWorldToObjectMatrix, float4(inPosWS, 1.0)).xyz;
    const float noiseVal = _Wiwiw_DensityTexture.SampleLevel(sampler_Wiwiw_DensityTexture, noisePos, 0.0).r;

    const float remapMin = lerp(0.0, 1.0 - _Wiwiw_CloudsMidpoint, _Wiwiw_CloudsContrast);
    const float remapMax = lerp(1.0, 1.0 - _Wiwiw_CloudsMidpoint, _Wiwiw_CloudsContrast);
    const float remapVal = smoothstep(remapMin, remapMax, noiseVal);

    return remapVal;
}

float Wiwiw_GetClouds(in float3 inPosWS)
{
    const float cloudsBlend = Wiwiw_GetCloudsBlend(inPosWS);
    const float cloudsValue = Wiwiw_GetCloudsValue(inPosWS);

    return cloudsBlend * cloudsValue;
}

void Wiwiw_GetClouds_float3(in float3 inPosWS, out float outValue)
{
    outValue = Wiwiw_GetClouds(inPosWS);
}

// --------------------------------------------------------

#endif // WIWIW_GET_CLOUDS_INCLUDED
