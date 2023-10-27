#ifndef WIWIW_LIB_CLOUDS_INCLUDED
#define WIWIW_LIB_CLOUDS_INCLUDED

// --------------------------------------------------------
// Includes
// --------------------------------------------------------

#include "../../../Utilities/Shaders/Library/LibCommon.hlsl"

// --------------------------------------------------------
// Density Texture
// --------------------------------------------------------

static const uint WIWIW_LIB_CLOUDS_NUM_DENSITY_LAYERS = 4;

// --------------------------------------------------------

Texture3D<float> _Wiwiw_LibClouds_DensityTexture;

SamplerState sampler_Wiwiw_LibClouds_DensityTexture;

float _Wiwiw_LibClouds_DensityMultiply[WIWIW_LIB_CLOUDS_NUM_DENSITY_LAYERS];

float _Wiwiw_LibClouds_DensityContrast[WIWIW_LIB_CLOUDS_NUM_DENSITY_LAYERS];

float _Wiwiw_LibClouds_DensityMidpoint[WIWIW_LIB_CLOUDS_NUM_DENSITY_LAYERS];

// --------------------------------------------------------

float Wiwiw_LibClouds_SampleDensityTexture(in float3 inPos)
{
    float result = 0.5;

    for (uint i = 0; i < WIWIW_LIB_CLOUDS_NUM_DENSITY_LAYERS; i++)
    {
        const float3 texturePos = inPos * _Wiwiw_LibClouds_DensityMultiply[i];
        const float textureVal = 1.0 - _Wiwiw_LibClouds_DensityTexture.SampleLevel(sampler_Wiwiw_LibClouds_DensityTexture, texturePos, 0.0).r;

        const float smoothstepMin = lerp(0.0, 1.0 - _Wiwiw_LibClouds_DensityMidpoint[i], _Wiwiw_LibClouds_DensityContrast[i]);
        const float smoothstepMax = lerp(1.0, 1.0 - _Wiwiw_LibClouds_DensityMidpoint[i], _Wiwiw_LibClouds_DensityContrast[i]);
        const float smoothstepVal = smoothstep(smoothstepMin, smoothstepMax, textureVal);

        result = Wiwiw_BlendOverlay(result, smoothstepVal);
    }

    return result;
}

// --------------------------------------------------------
// Gradient Texture
// --------------------------------------------------------

static const uint WIWIW_LIB_CLOUDS_NUM_GRADIENT_POINTS = 4;

// --------------------------------------------------------

float _Wiwiw_LibClouds_GradientPoints[WIWIW_LIB_CLOUDS_NUM_GRADIENT_POINTS];

float _Wiwiw_LibClouds_GradientColors[WIWIW_LIB_CLOUDS_NUM_GRADIENT_POINTS];

// --------------------------------------------------------

float Wiwiw_LibClouds_SampleGradientTexture(in float inPos)
{
    float result = 0.0;

    for (uint i = 0; i < WIWIW_LIB_CLOUDS_NUM_GRADIENT_POINTS - 1; i++)
    {
        const float startPoint = _Wiwiw_LibClouds_GradientPoints[i];
        const float finalPoint = _Wiwiw_LibClouds_GradientPoints[i + 1];
        const float isInInterval = inPos >= startPoint && inPos <= finalPoint;

        const float startColor = _Wiwiw_LibClouds_GradientColors[i];
        const float finalColor = _Wiwiw_LibClouds_GradientColors[i + 1];
        const float colorOfInterval = lerp(startColor, finalColor, smoothstep(startPoint, finalPoint, inPos));

        result += isInInterval * colorOfInterval;
    }

    return result;
}

// --------------------------------------------------------
// Clouds Main
// --------------------------------------------------------

float _Wiwiw_LibClouds_CloudsHeightMin;

float _Wiwiw_LibClouds_CloudsHeightMax;

float _Wiwiw_LibClouds_CloudsSampleStepDensity;

float _Wiwiw_LibClouds_CloudsSampleFullDistance;

float _Wiwiw_LibClouds_CloudsSampleNumberFlt;

float _Wiwiw_LibClouds_CloudsSampleNumberRcp;

// --------------------------------------------------------

float Wiwiw_LibClouds_SampleCloudsDensity(in float3 inPosWS)
{
    float result = 1.0;

    const float3 densityPos = inPosWS;
    result *= Wiwiw_LibClouds_SampleDensityTexture(densityPos);

    const float gradientPos = Wiwiw_InverseLerp(_Wiwiw_LibClouds_CloudsHeightMin, _Wiwiw_LibClouds_CloudsHeightMax, inPosWS.y);
    result *= Wiwiw_LibClouds_SampleGradientTexture(gradientPos);

    return result;
}

void Wiwiw_GetIntegrateCloudsInterval(in float3 inPosWS, in float3 inDirWS, out float3 outStartPosWS, out float3 outFinalPosWS)
{
    bool isValid = true;

    const float lowerPlanePosWS = _Wiwiw_LibClouds_CloudsHeightMin;
    const float upperPlanePosWS = _Wiwiw_LibClouds_CloudsHeightMax;
    isValid = (isValid) && (inPosWS.y < lowerPlanePosWS || inPosWS.y > upperPlanePosWS);

    const float3 lowerIntersectPosWS = Wiwiw_GetRayIntersectPlaneY(inPosWS, inDirWS, lowerPlanePosWS);
    const float3 upperIntersectPosWS = Wiwiw_GetRayIntersectPlaneY(inPosWS, inDirWS, upperPlanePosWS);
    isValid = (isValid) && (!Wiwiw_IsNan(lowerIntersectPosWS) && !Wiwiw_IsNan(upperIntersectPosWS));

    const float lowerIntersectTimeWS = Wiwiw_GetRayTime(inPosWS, inDirWS, lowerIntersectPosWS);
    const float upperIntersectTimeWS = Wiwiw_GetRayTime(inPosWS, inDirWS, upperIntersectPosWS);
    isValid = (isValid) && (lowerIntersectTimeWS >= 0.0 && upperIntersectTimeWS >= 0.0);

    const float3 defaultSamplePosWS = inPosWS;
    const float3 nearestSamplePosWS = lowerIntersectTimeWS < upperIntersectTimeWS ? lowerIntersectPosWS : upperIntersectPosWS;
    isValid = (isValid) && (!Wiwiw_IsNan(nearestSamplePosWS));

    // outStartPosWS = isValid ? nearestSamplePosWS : defaultSamplePosWS;
    // outFinalPosWS = outStartPosWS + inDirWS * _Wiwiw_LibClouds_CloudsSampleFullDistance;

    outFinalPosWS = isValid ? nearestSamplePosWS : defaultSamplePosWS;
    outStartPosWS = outFinalPosWS + inDirWS * _Wiwiw_LibClouds_CloudsSampleFullDistance;
}

// --------------------------------------------------------

float Wiwiw_LibClouds_IntegrateClouds(in float3 inPosWS, in float3 inDirWS)
{
    float3 sampleStartPosWS = 0.0;
    float3 sampleFinalPosWS = 0.0;
    Wiwiw_GetIntegrateCloudsInterval(inPosWS, inDirWS, sampleStartPosWS, sampleFinalPosWS);

    float light = 1.0;

    for (float i = 0.0; i < _Wiwiw_LibClouds_CloudsSampleNumberFlt; i++)
    {
        const float3 samplePosWS = lerp(sampleStartPosWS, sampleFinalPosWS, i * _Wiwiw_LibClouds_CloudsSampleNumberRcp);
        const float sampleValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        light *= exp(-sampleValWS * _Wiwiw_LibClouds_CloudsSampleStepDensity);
    }

    return 1.0 - light;
}

// --------------------------------------------------------

#endif // WIWIW_LIB_CLOUDS_INCLUDED
