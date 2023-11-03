#ifndef WIWIW_LIB_SKY_INCLUDED
#define WIWIW_LIB_SKY_INCLUDED

// --------------------------------------------------------
// Includes
// --------------------------------------------------------

#include "../../../Utilities/Shaders/Library/LibCommon.hlsl"

// --------------------------------------------------------
// Skybox Utilities
// --------------------------------------------------------

Texture2DArray _SkyboxTexture;

SamplerState sampler_SkyboxTexture;

Texture2D _StabilityTexture;

SamplerState sampler_StabilityTexture;

float _TextureBlending;

float _BentNormalScale;

float _BentNormalPower;

float _GradientPoint0;

float3 _GradientValue0;

float _GradientPoint1;

float3 _GradientValue1;

float _GradientPoint2;

float3 _GradientValue2;

float _GradientPoint3;

float3 _GradientValue3;

float _AmbientPoint0;

float3 _AmbientValue0;

float _AmbientPoint1;

float3 _AmbientValue1;

float3 _SunDir;

float3 _SunCol;

// --------------------------------------------------------

float3 GetGradientColor(float3 dirWS)
{
    float3 gradientColor = 0.0;

    const bool isInInterval0 = dirWS.y < _GradientPoint1;
    const float3 colorOfInterval0 = lerp(_GradientValue0, _GradientValue1, smoothstep(_GradientPoint0, _GradientPoint1, dirWS.y));
    gradientColor += isInInterval0 ? colorOfInterval0 : 0.0;

    const bool isInInterval1 = dirWS.y >= _GradientPoint1 && dirWS.y < _GradientPoint2;
    const float3 colorOfInterval1 = lerp(_GradientValue1, _GradientValue2, smoothstep(_GradientPoint1, _GradientPoint2, dirWS.y));
    gradientColor += isInInterval1 ? colorOfInterval1 : 0.0;

    const bool isInInterval2 = dirWS.y >= _GradientPoint2;
    const float3 colorOfInterval2 = lerp(_GradientValue2, _GradientValue3, smoothstep(_GradientPoint2, _GradientPoint3, dirWS.y));
    gradientColor += isInInterval2 ? colorOfInterval2 : 0.0;

    return gradientColor;
}

float3 GetAmbientColor(float3 dirWS)
{
    return lerp(_AmbientValue0, _AmbientValue1, smoothstep(_AmbientPoint0, _AmbientPoint1, dirWS.y));
}

float4 GetSunColor(float3 dirWS)
{
    float3 sunCol = 0.0;
    sunCol = 10.0 * _SunCol;
    sunCol = sunCol * rcp(sunCol + 1.0);

    float sunDiscFac = 0.0;
    sunDiscFac = dot(dirWS, -_SunDir);
    sunDiscFac = step(0.995, sunDiscFac);

    float sunGlowFac = 0.0;
    // sunGlowFac = dot(dirWS, -_SunDir);
    // sunGlowFac = saturate(mad(sunGlowFac, 0.5, 0.5));
    // sunGlowFac = 0.5 * pow(sunGlowFac, 32.0);

    return float4(sunCol, max(sunDiscFac, sunGlowFac));
}

float4 GetCloudsColor(float3 dirWS)
{
    const float4 cloudsData = Wiwiw_SampleCubemap(_SkyboxTexture, sampler_SkyboxTexture, dirWS);
    const float3 cloudsNormal = mad(cloudsData.rgb, 2.0, -1.0);
    const float cloudsAlpha = cloudsData.a;

    float3 cloudsColor = 0.0;

    float cloudsSunFactor = 0.0;
    cloudsSunFactor = dot(cloudsNormal, -_SunDir);
    cloudsSunFactor = saturate(mad(cloudsSunFactor, 0.5, 0.5));
    cloudsSunFactor = _BentNormalScale * pow(cloudsSunFactor, _BentNormalPower);
    cloudsSunFactor = cloudsSunFactor * rcp(cloudsSunFactor + 1.0);

    cloudsColor += cloudsSunFactor * _SunCol;

    float cloudsSkyFactor = 0.0;
    cloudsSkyFactor = dot(cloudsNormal, float3(0.0, 1.0, 0.0));
    cloudsSkyFactor = mad(cloudsSkyFactor, 0.5, 0.5);

    cloudsColor += GetAmbientColor(cloudsSkyFactor);

    return float4(cloudsColor, cloudsAlpha);
}

float3 GetSkyboxColor(float3 worldDir)
{
    const float4 sunColor = GetSunColor(worldDir);
    const float4 cloudsColor = GetCloudsColor(worldDir);

    float3 output = GetGradientColor(worldDir);
    output = lerp(output, sunColor.rgb, sunColor.a);
    output = lerp(output, cloudsColor.rgb, cloudsColor.a);
    output = lerp(output, Wiwiw_SampleSpheremap(_StabilityTexture, sampler_StabilityTexture, worldDir).xyz, _TextureBlending);
    return output;
}

// --------------------------------------------------------

#endif // WIWIW_LIB_SKY_INCLUDED
