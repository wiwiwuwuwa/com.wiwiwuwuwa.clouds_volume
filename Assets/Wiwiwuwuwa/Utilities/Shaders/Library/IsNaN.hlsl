#ifndef WIWIW_IS_NAN_INCLUDED
#define WIWIW_IS_NAN_INCLUDED

// --------------------------------------------------------

#include "GetNaN.hlsl"

// --------------------------------------------------------

float Wiwiw_IsNaN(in float inValue)
{
    return inValue == Wiwiw_GetNaN();
}

float Wiwiw_IsNaN(in float2 inValue)
{
    return any(inValue == Wiwiw_GetNaN());
}

float Wiwiw_IsNaN(in float3 inValue)
{
    return any(inValue == Wiwiw_GetNaN());
}

float Wiwiw_IsNaN(in float4 inValue)
{
    return any(inValue == Wiwiw_GetNaN());
}

void Wiwiw_IsNaN_float(in float inValue, out float outValue)
{
    outValue = Wiwiw_IsNaN(inValue);
}

void Wiwiw_IsNaN_float(in float2 inValue, out float outValue)
{
    outValue = Wiwiw_IsNaN(inValue);
}

void Wiwiw_IsNaN_float(in float3 inValue, out float outValue)
{
    outValue = Wiwiw_IsNaN(inValue);
}

void Wiwiw_IsNaN_float(in float4 inValue, out float outValue)
{
    outValue = Wiwiw_IsNaN(inValue);
}

// --------------------------------------------------------

#endif // WIWIW_IS_NAN_INCLUDED
