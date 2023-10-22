#ifndef WIWIW_LIB_COMMON_INCLUDED
#define WIWIW_LIB_COMMON_INCLUDED

// --------------------------------------------------------
// Math
// --------------------------------------------------------

uint3 Wiwiw_Get3DFrom1D(in uint inCoord, in uint inWidth, in uint inHeight)
{
    uint3 outCoord = 0;
	outCoord.x = inCoord % inWidth;
	outCoord.y = (inCoord / inWidth) % inHeight;
	outCoord.z = inCoord / (inWidth * inHeight);
	return outCoord;
}

// --------------------------------------------------------

#endif // WIWIW_LIB_COMMON_INCLUDED
