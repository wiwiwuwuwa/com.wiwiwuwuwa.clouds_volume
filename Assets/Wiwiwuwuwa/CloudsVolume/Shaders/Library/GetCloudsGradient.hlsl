#ifndef WIWIW_GET_CLOUDS_GRADIENT_INCLUDED
#define WIWIW_GET_CLOUDS_GRADIENT_INCLUDED

// --------------------------------------------------------

// inPosition: gradient position in range [0, 1]
// inGradientParams.x: clouds fade in start pos in range [0, 1]
// inGradientParams.y: clouds fade in final pos in range [0, 1]
// inGradientParams.z: clouds fade out start pos in range [0, 1]
// inGradientParams.w: clouds fade out final pos in range [0, 1]
float Wiwiw_GetCloudsGradient(in float inPosition, in float4 inGradientParams)
{
	float result = 0.0;

	const float isInInterval0 = inPosition < inGradientParams.x;
	const float valueOfInterval0 = 0.0;
	result += isInInterval0 * valueOfInterval0;

	const float isInInterval1 = inPosition >= inGradientParams.x && inPosition < inGradientParams.y;
	const float valueOfInterval1 = smoothstep(inGradientParams.x, inGradientParams.y, inPosition);
	result += isInInterval1 * valueOfInterval1;

	const float isInInterval2 = inPosition >= inGradientParams.y && inPosition < inGradientParams.z;
	const float valueOfInterval2 = 1.0;
	result += isInInterval2 * valueOfInterval2;

	const float isInInterval3 = inPosition >= inGradientParams.z && inPosition < inGradientParams.w;
	const float valueOfInterval3 = smoothstep(inGradientParams.w, inGradientParams.z, inPosition);
	result += isInInterval3 * valueOfInterval3;

	const float isInInterval4 = inPosition >= inGradientParams.w;
	const float valueOfInterval4 = 0.0;
	result += isInInterval4 * valueOfInterval4;

	return result;
}

void Wiwiw_GetCloudsGradient_float(in float inPosition, in float4 inGradientParams, out float outValue)
{
	outValue = Wiwiw_GetCloudsGradient(inPosition, inGradientParams);
}

// --------------------------------------------------------

#endif // WIWIW_GET_CLOUDS_GRADIENT_INCLUDED
