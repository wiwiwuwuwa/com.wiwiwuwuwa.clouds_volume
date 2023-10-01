#ifndef WIWIW_GET_FOG_EXP2_INCLUDED
#define WIWIW_GET_FOG_EXP2_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetFogExp2(in float inDensity, in float inDistance)
{
    return exp2(-pow(inDensity * inDistance, 2.0));
}

void Wiwiw_GetFogExp2_float(in float inDensity, in float inDistance, out float outValue)
{
    outValue = Wiwiw_GetFogExp2(inDensity, inDistance);
}

// --------------------------------------------------------

#endif // WIWIW_GET_FOG_EXP2_INCLUDED
