#ifndef WIWIW_GET_NAN_INCLUDED
#define WIWIW_GET_NAN_INCLUDED

// --------------------------------------------------------

float Wiwiw_GetNaN()
{
    return asfloat(0xFFFFFFFF);
}

void Wiwiw_GetNaN_float(out float outValue)
{
    outValue = Wiwiw_GetNaN();
}

// --------------------------------------------------------

#endif // WIWIW_GET_NAN_INCLUDED
