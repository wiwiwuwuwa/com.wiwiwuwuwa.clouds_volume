#ifndef WIWIW_GET_RAY_TIME_INCLUDED
#define WIWIW_GET_RAY_TIME_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetRayTime(in float3 inPos, in float3 inRayPos, in float3 inRayDir)
{
    return dot(inPos - inRayPos, inRayDir) * rcp(dot(inRayDir, inRayDir));
}

void Wiwiw_GetRayTime_float(in float3 inPos, in float3 inRayPos, in float3 inRayDir, out float outValue)
{
    outValue = Wiwiw_GetRayTime(inPos, inRayPos, inRayDir);
}

// --------------------------------------------------------

#endif // WIWIW_GET_RAY_TIME_INCLUDED
