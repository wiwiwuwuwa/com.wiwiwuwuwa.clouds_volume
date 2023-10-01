#ifndef WIWIW_GET_GLOBAL_BUFFER_INDEX_3D_INCLUDED
#define WIWIW_GET_GLOBAL_BUFFER_INDEX_3D_INCLUDED

// --------------------------------------------------------

#include "Get3DFrom1D.hlsl"
#include "GetGlobalBufferIndex1D.hlsl"

// --------------------------------------------------------

uint3 Wiwiw_GetGlobalBufferIndex3D(in uint inDispatchThreadID)
{
    const uint globalThreadIndex = Wiwiw_GetGlobalBufferIndex1D(inDispatchThreadID);
    return Wiwiw_Get3DFrom1D(globalThreadIndex, _Wiwiw_BufferInfo.x, _Wiwiw_BufferInfo.y);
}

void Wiwiw_GetGlobalBufferIndex3D_float(in uint inDispatchThreadID, out uint3 outBufferIndex)
{
    outBufferIndex = Wiwiw_GetGlobalBufferIndex3D(inDispatchThreadID);
}

// --------------------------------------------------------

#endif // WIWIW_GET_GLOBAL_BUFFER_INDEX_3D_INCLUDED
