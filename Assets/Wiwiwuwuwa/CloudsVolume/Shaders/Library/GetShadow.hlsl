#ifndef WIWIW_GET_SHADOW_INCLUDED
#define WIWIW_GET_SHADOW_INCLUDED

// --------------------------------------------------------

Texture3D<float> _Wiwiw_ShadowsTexture;

SamplerState sampler_Wiwiw_ShadowsTexture;

float4x4 _Wiwiw_ShadowsWorldToObjectMatrix;

float3 _Wiwiw_ShadowDirection;

// --------------------------------------------------------

float Wiwiw_GetShadow(in float3 inPosWS)
{
    const float3 posOS = mul(_Wiwiw_ShadowsWorldToObjectMatrix, float4(inPosWS, 1.0)).xyz;
    return _Wiwiw_ShadowsTexture.SampleLevel(sampler_Wiwiw_ShadowsTexture, posOS, 0.0).r;
}

// --------------------------------------------------------

#endif // WIWIW_GET_SHADOW_INCLUDED
