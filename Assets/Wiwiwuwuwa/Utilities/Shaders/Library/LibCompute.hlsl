#ifndef WIWIW_LIB_COMPUTE_INCLUDED
#define WIWIW_LIB_COMPUTE_INCLUDED

// --------------------------------------------------------
// Includes
// --------------------------------------------------------

#include "LibCommon.hlsl"

// --------------------------------------------------------
// Wavefront size
// --------------------------------------------------------

#if defined(WIWIW_WAVEFRONT_SIZE_16)
    #define WIWIW_NUMTHREADS [numthreads(16, 1, 1)]
#elif defined(WIWIW_WAVEFRONT_SIZE_32)
    #define WIWIW_NUMTHREADS [numthreads(32, 1, 1)]
#elif defined(WIWIW_WAVEFRONT_SIZE_64)
    #define WIWIW_NUMTHREADS [numthreads(64, 1, 1)]
#else
    #error "WIWIW_WAVEFRONT_SIZE is not defined"
#endif

// --------------------------------------------------------
// Thread index
// --------------------------------------------------------

int3 _Wiwiw_BufferSizeInt3;

float3 _Wiwiw_BufferSizeFlt3;

float3 _Wiwiw_BufferSizeRcp3;

int _Wiwiw_ThreadOffset;

// ------------------------------------

uint Wiwiw_GetThreadIndex1D(in uint3 inDispatchThreadID)
{
    return _Wiwiw_ThreadOffset + inDispatchThreadID.x;
}

uint3 Wiwiw_GetThreadIndex3D(in uint3 inDispatchThreadID)
{
    const uint threadIndex1D = Wiwiw_GetThreadIndex1D(inDispatchThreadID);
    return Wiwiw_Get3DFrom1D(threadIndex1D, _Wiwiw_BufferSizeInt3.x, _Wiwiw_BufferSizeInt3.y);
}

float3 Wiwiw_GetThreadCoord3D(in uint3 inDispatchThreadID)
{
    const uint3 threadIndex3D = Wiwiw_GetThreadIndex3D(inDispatchThreadID);
    return (threadIndex3D + 0.5) * _Wiwiw_BufferSizeRcp3;
}

// --------------------------------------------------------

#endif // WIWIW_LIB_COMPUTE_INCLUDED
