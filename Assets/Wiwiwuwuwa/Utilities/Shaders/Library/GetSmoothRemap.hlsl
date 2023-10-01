#ifndef WIWIW_GET_SMOOTH_REMAP_INCLUDED
#define WIWIW_GET_SMOOTH_REMAP_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetSmoothRemap(in float inX, in float inOldMin, in float inOldMax, in float inNewMin, in float inNewMax)
{
	return lerp(inNewMin, inNewMax, smoothstep(inOldMin, inOldMax, inX));
}

void Wiwiw_GetSmoothRemap_float(in float inX, in float inOldMin, in float inOldMax, in float inNewMin, in float inNewMax, out float outX)
{
	outX = Wiwiw_GetSmoothRemap(inX, inOldMin, inOldMax, inNewMin, inNewMax);
}

// --------------------------------------------------------

#endif // WIWIW_GET_SMOOTH_REMAP_INCLUDED
