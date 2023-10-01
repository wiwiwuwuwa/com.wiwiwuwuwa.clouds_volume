#ifndef WIWIW_WAVE_FRONT_MULTI_COMPILE_INCLUDED
#define WIWIW_WAVE_FRONT_MULTI_COMPILE_INCLUDED

// --------------------------------------------------------

#if defined(WIWIW_THREADS_PER_WAVE_16)
	#define WIWIW_NUMTHREADS [numthreads(16, 1, 1)]
#elif defined(WIWIW_THREADS_PER_WAVE_32)
	#define WIWIW_NUMTHREADS [numthreads(32, 1, 1)]
#elif defined(WIWIW_THREADS_PER_WAVE_64)
	#define WIWIW_NUMTHREADS [numthreads(64, 1, 1)]
#else
    #error "WIWIW_THREADS_PER_WAVE is not defined"
#endif

// --------------------------------------------------------

#endif // WIWIW_WAVE_FRONT_MULTI_COMPILE_INCLUDED
