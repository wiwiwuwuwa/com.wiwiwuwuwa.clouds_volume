#ifndef WIWIW_WAVE_FRONT_GLOBAL_BUFFER_INCLUDED
#define WIWIW_WAVE_FRONT_GLOBAL_BUFFER_INCLUDED

// --------------------------------------------------------

// x: buffer size x
// y: buffer size y
// z: buffer size z
// w: linear buffer size
int4 _Wiwiw_BufferInfo;

// x: buffer size x
// y: buffer size y
// z: buffer size z
// w: linear buffer size
float4 _Wiwiw_BufferInfoVal;

// x: 1 / buffer size x
// y: 1 / buffer size y
// z: 1 / buffer size z
// w: 1 / linear buffer size
float4 _Wiwiw_BufferInfoInv;

// x: thread offset
// y: threads count
// z: groups count
// w: unused
int4 _Wiwiw_ThreadInfo;

// --------------------------------------------------------

#endif // WIWIW_WAVE_FRONT_GLOBAL_BUFFER_INCLUDED
