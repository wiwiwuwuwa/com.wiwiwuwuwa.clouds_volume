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
// Clouds Density
// --------------------------------------------------------

float _Wiwiw_LibClouds_CloudsHeightMin;

float _Wiwiw_LibClouds_CloudsHeightMax;

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

// --------------------------------------------------------
// Bent Normals
// --------------------------------------------------------

static const float3 WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[] =
{
    float3(0.125, 0.256, 0.111),
    float3(-0.234, 0.456, -0.789),
    float3(0.345, -0.567, 0.678),
    float3(-0.456, -0.789, -0.123),
    float3(0.567, 0.234, -0.890),
    float3(-0.678, 0.901, 0.345),
    float3(0.789, -0.012, -0.456),
    float3(-0.901, -0.345, 0.567),
    float3(0.234, 0.789, -0.456),
    float3(-0.567, 0.123, 0.890),
    float3(0.901, -0.678, -0.345),
    float3(-0.012, -0.456, 0.789),
    float3(0.345, 0.567, -0.234),
    float3(-0.789, -0.123, 0.456),
    float3(0.456, -0.789, -0.567),
    float3(-0.678, 0.234, 0.901),
    float3(0.567, 0.890, 0.012),
    float3(-0.901, -0.345, -0.123),
    float3(0.123, 0.456, 0.789),
    float3(-0.456, 0.789, -0.012),
    float3(0.678, -0.901, 0.345),
    float3(-0.789, 0.012, -0.456),
    float3(0.901, 0.345, 0.567),
    float3(-0.234, -0.789, -0.456),
    float3(0.567, -0.123, 0.890),
    float3(-0.901, 0.678, -0.345),
    float3(0.012, 0.456, 0.789),
    float3(-0.345, -0.567, -0.234),
    float3(0.789, 0.123, 0.456),
    float3(-0.456, 0.789, -0.567),
    float3(0.678, -0.234, 0.901),
    float3(-0.567, -0.890, 0.012),
    float3(0.901, 0.345, -0.123)
};

// --------------------------------------------------------

float3 Wiwiw_LibClouds_SampleCloudsNormals(in float3 inPosWS)
{
    float3 result = 0.0;

    for (uint i = 0; i < 8; i++)
    {
        const float3 samplePosWS = inPosWS + WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * 4.0;
        const float sampleValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        result += WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * (1.0 - sampleValWS);
    }

        for (uint i = 0; i < 8; i++)
    {
        const float3 samplePosWS = inPosWS + WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * 8.0;
        const float sampleValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        result += WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * (1.0 - sampleValWS);
    }

        for (uint i = 0; i < 8; i++)
    {
        const float3 samplePosWS = inPosWS + WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * 16.0;
        const float sampleValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        result += WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * (1.0 - sampleValWS);
    }

        for (uint i = 0; i < 8; i++)
    {
        const float3 samplePosWS = inPosWS + WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * 32.0;
        const float sampleValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        result += WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i] * (1.0 - sampleValWS);
    }

    return result * rcp(8.0 * 4.0);

}

// --------------------------------------------------------
// Clouds Integration
// --------------------------------------------------------

float _Wiwiw_LibClouds_CloudsSampleStepDensity;

float _Wiwiw_LibClouds_CloudsSampleFullDistance;

float _Wiwiw_LibClouds_CloudsSampleNumberFlt;

float _Wiwiw_LibClouds_CloudsSampleNumberRcp;

// --------------------------------------------------------

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

    outFinalPosWS = isValid ? nearestSamplePosWS : defaultSamplePosWS;
    outStartPosWS = outFinalPosWS + inDirWS * _Wiwiw_LibClouds_CloudsSampleFullDistance;
}

// --------------------------------------------------------

float4 Wiwiw_LibClouds_IntegrateClouds(in float3 inPosWS, in float3 inDirWS)
{
    float3 sampleStartPosWS = 0.0;
    float3 sampleFinalPosWS = 0.0;
    Wiwiw_GetIntegrateCloudsInterval(inPosWS, inDirWS, sampleStartPosWS, sampleFinalPosWS);

    float3 color = 0.0;
    float alpha = 1.0;

    for (float i = 0.0; i < _Wiwiw_LibClouds_CloudsSampleNumberFlt; i++)
    {
        const float3 samplePosWS = lerp(sampleStartPosWS, sampleFinalPosWS, i * _Wiwiw_LibClouds_CloudsSampleNumberRcp);
        const float3 sampleColorValWS = Wiwiw_LibClouds_SampleCloudsNormals(samplePosWS);
        const float sampleAlphaValWS = Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        const float fogFactor = exp(-sampleAlphaValWS * _Wiwiw_LibClouds_CloudsSampleStepDensity);

        color = lerp(sampleColorValWS, color, fogFactor);
        alpha = lerp(0.0, alpha, fogFactor);
    }

    return float4(mad(color, 0.5, 0.5), 1.0 - alpha);
}

// --------------------------------------------------------

#endif // WIWIW_LIB_CLOUDS_INCLUDED
