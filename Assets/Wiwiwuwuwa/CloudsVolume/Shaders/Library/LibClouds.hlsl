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

static const uint WIWIW_LIB_CLOUDS_NUM_BENT_NORMAL_SAMPLES = 32;

static const float3 WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[WIWIW_LIB_CLOUDS_NUM_BENT_NORMAL_SAMPLES] =
{
    float3(0.297923002725570152, -0.267826414759111620, 0.267230598356107074),
    float3(-0.151378121456606618, -0.275610485033434827, 0.450742981360957973),
    float3(0.088244796597815423, -0.097850424192025928, 0.018798780300073488),
    float3(0.247591112022557597, 0.497829755980466915, 0.015735105104679702),
    float3(0.258981342066747278, 0.598677636837941174, -0.001571788760639811),
    float3(0.740669571778249436, 0.466806843042884501, -0.011064346871848936),
    float3(0.202923163462026712, 0.292079965412393339, -0.757215012065673099),
    float3(0.225714329955814780, -0.813642236240714189, -0.233815814071807571),
    float3(-0.182761308952665069, 0.400292619875000699, 0.547973244906096779),
    float3(0.757219928548374943, 0.017969035761496065, -0.460022484261721654),
    float3(0.458654516951092994, 0.207728071826299265, -0.460249515604406689),
    float3(-0.247931080301704382, 0.632891501576168802, 0.425932948488421392),
    float3(0.543406302935566798, -0.660509314056207497, 0.190533670869758004),
    float3(0.140020553872736819, -0.039316902618635151, 0.471021189415228470),
    float3(0.385309560604869095, 0.308197547991402920, -0.681486271168404545),
    float3(0.160151963875459824, 0.109615345359506788, -0.876816419164191285),
    float3(0.740012200975919243, -0.448855912521509803, -0.108945094522297073),
    float3(0.126353469961046738, -0.063756467319316543, -0.751722842290604265),
    float3(-0.038915735242728684, 0.873215435712065502, 0.090251785756331707),
    float3(0.647633343354084845, -0.582642160528055508, 0.180295942266405357),
    float3(0.355860790964780183, -0.500827814219730216, -0.521085001544897564),
    float3(-0.409518235555424048, 0.192397740559958980, 0.222287211412884239),
    float3(-0.802573802968168715, -0.017382589493514570, -0.344364478982383482),
    float3(0.060937049268912018, -0.096859958562966753, -0.347930954519182711),
    float3(-0.444298113710638320, 0.107510887538058320, -0.506679664778970973),
    float3(0.609601120832131471, 0.383792782800333798, 0.531469590740677011),
    float3(-0.005148213299221858, 0.313563582147985187, -0.861082437683459267),
    float3(-0.137650875229570119, 0.066608519992689019, -0.210548077670651756),
    float3(-0.293505970464687904, -0.583083898475598827, 0.354739925176579041),
    float3(-0.679448166062800563, 0.342835008650932749, 0.238328076757100743),
    float3(-0.172777371614814851, 0.333405667656024196, 0.433278915489621996),
    float3(0.631365626056101692, 0.123465684603655285, -0.426670073175216436)
};

// --------------------------------------------------------

float _Wiwiw_LibClouds_BentNormalScale;

// --------------------------------------------------------

float3 Wiwiw_LibClouds_SampleCloudsNormals(in float3 inPosWS)
{
    float3 bentNormal = 0.0;

    for (uint i = 0; i < WIWIW_LIB_CLOUDS_NUM_BENT_NORMAL_SAMPLES; i++)
    {
        const float3 samplePosOS = WIWIW_LIB_CLOUDS_BENT_NORMAL_SAMPLES[i];
        const float3 samplePosWS = inPosWS + samplePosOS * _Wiwiw_LibClouds_BentNormalScale;
        const float1 sampleValWS = 1.0 - Wiwiw_LibClouds_SampleCloudsDensity(samplePosWS);

        bentNormal += samplePosOS * sampleValWS.x;
    }

    bentNormal = 4.0 * bentNormal * rcp(WIWIW_LIB_CLOUDS_NUM_BENT_NORMAL_SAMPLES);
    bentNormal = sign(bentNormal) * abs(bentNormal) * rcp(abs(bentNormal) + 1.0);

    return bentNormal;
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

    color = mad(color, 0.5, 0.5);
    alpha = 1.0 - alpha;

    return float4(color, alpha);
}

// --------------------------------------------------------

#endif // WIWIW_LIB_CLOUDS_INCLUDED
