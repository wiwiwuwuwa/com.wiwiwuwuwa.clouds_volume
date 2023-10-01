#ifndef WIWIW_GET_GLOBAL_BUFFER_INDEX_1D_INCLUDED
#define WIWIW_GET_GLOBAL_BUFFER_INDEX_1D_INCLUDED

// --------------------------------------------------------

#include "WaveFrontGlobalBuffer.hlsl"

// --------------------------------------------------------

uint Wiwiw_GetGlobalBufferIndex1D(in uint inDispatchThreadID)
{
	return _Wiwiw_ThreadInfo.x + inDispatchThreadID;
}

void Wiwiw_GetGlobalBufferIndex1D_float(in uint inDispatchThreadID, out uint outBufferIndex)
{
	outBufferIndex = Wiwiw_GetGlobalBufferIndex1D(inDispatchThreadID);
}

// --------------------------------------------------------

#endif // WIWIW_GET_GLOBAL_BUFFER_INDEX_1D_INCLUDED
