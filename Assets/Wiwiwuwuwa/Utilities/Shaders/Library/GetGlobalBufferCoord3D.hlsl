#ifndef WIWIW_GET_GLOBAL_BUFFER_COORD_3D_INCLUDED
#define WIWIW_GET_GLOBAL_BUFFER_COORD_3D_INCLUDED

// --------------------------------------------------------

#include "Get3DFrom1D.hlsl"
#include "GetGlobalBufferIndex3D.hlsl"

// --------------------------------------------------------

float3 Wiwiw_GetGlobalBufferCoord3D(in uint inDispatchThreadID)
{
    const uint3 bufferIndex = Wiwiw_GetGlobalBufferIndex3D(inDispatchThreadID);
    return (bufferIndex + 0.5) * _Wiwiw_BufferInfoInv.xyz;
}

void Wiwiw_GetGlobalBufferCoord3D_float(in uint inDispatchThreadID, out float3 outBufferCoord)
{
    outBufferCoord = Wiwiw_GetGlobalBufferCoord3D(inDispatchThreadID);
}

// --------------------------------------------------------

#endif // WIWIW_GET_GLOBAL_BUFFER_COORD_3D_INCLUDED
