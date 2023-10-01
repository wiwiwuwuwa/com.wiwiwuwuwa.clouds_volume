#ifndef WIWIW_GET_POINT_ON_CUBEMAP_INCLUDED
#define WIWIW_GET_POINT_ON_CUBEMAP_INCLUDED

// --------------------------------------------------------

float3 Wiwiw_GetPointOnCubemap(in float2 inSideCoord, in uint inSideIndex)
{
    switch (inSideIndex)
    {
        case 0: return float3(1.0, inSideCoord.y, 1.0 - inSideCoord.x); // +x
        case 1: return float3(0.0, inSideCoord.y, inSideCoord.x); // -x
        case 2: return float3(inSideCoord.x, 1.0, 1.0 - inSideCoord.y); // +y
        case 3: return float3(inSideCoord.x, 0.0, inSideCoord.y); // -y
        case 4: return float3(inSideCoord.x, inSideCoord.y, 1.0); // +z
        case 5: return float3(1.0 - inSideCoord.x, inSideCoord.y, 0.0); // -z
        default: return 0.5;
    }
}

// --------------------------------------------------------

#endif // WIWIW_GET_POINT_ON_CUBEMAP_INCLUDED
