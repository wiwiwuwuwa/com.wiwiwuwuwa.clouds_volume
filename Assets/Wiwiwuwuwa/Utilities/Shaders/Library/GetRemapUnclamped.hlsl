#ifndef WIWIW_GET_REMAP_UNCLAMPED_INCLUDED
#define WIWIW_GET_REMAP_UNCLAMPED_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetRemapUnclamped(in float inX, in float inOldMin, in float inOldMax, in float inNewMin, in float inNewMax)
{
	return lerp(inNewMin, inNewMax, (inX - inOldMin) / (inOldMax - inOldMin));
}

float2 Wiwiw_GetRemapUnclamped(in float2 inX, in float2 inOldMin, in float2 inOldMax, in float2 inNewMin, in float2 inNewMax)
{
	return lerp(inNewMin, inNewMax, (inX - inOldMin) / (inOldMax - inOldMin));
}

float3 Wiwiw_GetRemapUnclamped(in float3 inX, in float3 inOldMin, in float3 inOldMax, in float3 inNewMin, in float3 inNewMax)
{
	return lerp(inNewMin, inNewMax, (inX - inOldMin) / (inOldMax - inOldMin));
}

float4 Wiwiw_GetRemapUnclamped(in float4 inX, in float4 inOldMin, in float4 inOldMax, in float4 inNewMin, in float4 inNewMax)
{
	return lerp(inNewMin, inNewMax, (inX - inOldMin) / (inOldMax - inOldMin));
}

void Wiwiw_GetRemapUnclamped_float(in float inX, in float inOldMin, in float inOldMax, in float inNewMin, in float inNewMax, out float outX)
{
	outX = Wiwiw_GetRemapUnclamped(inX, inOldMin, inOldMax, inNewMin, inNewMax);
}

void Wiwiw_GetRemapUnclamped_float(in float2 inX, in float2 inOldMin, in float2 inOldMax, in float2 inNewMin, in float2 inNewMax, out float2 outX)
{
	outX = Wiwiw_GetRemapUnclamped(inX, inOldMin, inOldMax, inNewMin, inNewMax);
}

void Wiwiw_GetRemapUnclamped_float(in float3 inX, in float3 inOldMin, in float3 inOldMax, in float3 inNewMin, in float3 inNewMax, out float3 outX)
{
	outX = Wiwiw_GetRemapUnclamped(inX, inOldMin, inOldMax, inNewMin, inNewMax);
}

void Wiwiw_GetRemapUnclamped_float(in float4 inX, in float4 inOldMin, in float4 inOldMax, in float4 inNewMin, in float4 inNewMax, out float4 outX)
{
	outX = Wiwiw_GetRemapUnclamped(inX, inOldMin, inOldMax, inNewMin, inNewMax);
}

// --------------------------------------------------------

#endif // WIWIW_GET_REMAP_UNCLAMPED_INCLUDED