#ifndef WIWIW_GET_LINE_INTERSECT_PLANE_Y_INCLUDED
#define WIWIW_GET_LINE_INTERSECT_PLANE_Y_INCLUDED

// --------------------------------------------------------

#include "GetNaN.hlsl"

// --------------------------------------------------------

float3 Wiwiw_GetLineIntersectPlaneY(in float3 inLinePos, in float3 inLineDir, in float inPlaneY)
{
    const float isValid = (inLineDir.y != 0.0);

    const float t = (inPlaneY - inLinePos.y) * rcp(inLineDir.y);
    const float3 intersectPos = inLinePos + t * inLineDir;

    return isValid ? intersectPos : Wiwiw_GetNaN();
}

void Wiwiw_GetLineIntersectPlaneY_float(in float3 inLinePos, in float3 inLineDir, in float inPlaneY, out float3 outValue)
{
    outValue = Wiwiw_GetLineIntersectPlaneY(inLinePos, inLineDir, inPlaneY);
}

// --------------------------------------------------------

#endif // WIWIW_GET_LINE_INTERSECT_PLANE_Y_INCLUDED
