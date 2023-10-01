#ifndef WIWIW_GET_FADE_GRADIENT_INCLUDED
#define WIWIW_GET_FADE_GRADIENT_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetFadeGradient(in float inUnitPos, in float inGradientPoint0, in float inGradientValue0, in float inGradientPoint1, in float inGradientValue1)
{
	const float len = clamp(inUnitPos, inGradientPoint0, inGradientPoint1);
	const float val = lerp(inGradientValue0, inGradientValue1, smoothstep(inGradientPoint0, inGradientPoint1, len));

	return val;
}

float Wiwiw_GetFadeGradient(in float3 inUnitCubePos, in float inGradientPoint0, in float inGradientValue0, in float inGradientPoint1, in float inGradientValue1)
{
	return Wiwiw_GetFadeGradient(length(inUnitCubePos), inGradientPoint0, inGradientValue0, inGradientPoint1, inGradientValue1);
}

void Wiwiw_GetFadeGradient_float(in float inUnitPos, in float inGradientPoint0, in float inGradientValue0, in float inGradientPoint1, in float inGradientValue1, out float outValue)
{
	outValue = Wiwiw_GetFadeGradient(inUnitPos, inGradientPoint0, inGradientValue0, inGradientPoint1, inGradientValue1);
}

void Wiwiw_GetFadeGradient_float(in float3 inUnitCubePos, in float inGradientPoint0, in float inGradientValue0, in float inGradientPoint1, in float inGradientValue1, out float outValue)
{
	outValue = Wiwiw_GetFadeGradient(inUnitCubePos, inGradientPoint0, inGradientValue0, inGradientPoint1, inGradientValue1);
}

// --------------------------------------------------------

#endif // WIWIW_GET_FADE_GRADIENT_INCLUDED
