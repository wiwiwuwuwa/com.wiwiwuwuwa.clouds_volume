#ifndef WIWIW_GET_3D_FROM_1D_INCLUDED
#define WIWIW_GET_3D_FROM_1D_INCLUDED

// --------------------------------------------------------

uint3 Wiwiw_Get3DFrom1D(in uint inCoord, in uint inWidth, in uint inHeight)
{
	uint3 outCoord = 0;
	outCoord.x = inCoord % inWidth;
	outCoord.y = (inCoord / inWidth) % inHeight;
	outCoord.z = inCoord / (inWidth * inHeight);
	return outCoord;
}

void Wiwiw_Get3DFrom1D_uint(in uint inCoord, in uint inWidth, in uint inHeight, out uint3 outCoord)
{
	outCoord = Wiwiw_Get3DFrom1D(inCoord, inWidth, inHeight);
}

// --------------------------------------------------------

#endif // WIWIW_GET_3D_FROM_1D_INCLUDED
