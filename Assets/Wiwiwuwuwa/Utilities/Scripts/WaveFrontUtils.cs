using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;

namespace Wiwiwuwuwa.Utilities
{
    public static class WaveFrontUtils
    {
        // ----------------------------------------------------

        const int THREADS_PER_DISPATCH = 128 * 128;

        const string SHADER_KERNEL_NAME = "CSMain";

        const string SHADER_THREADS_PER_WAVE_KEYWORD = "WIWIW_THREADS_PER_WAVE";

        const string SHADER_BUFFER_INFO_PROPERTY = "_Wiwiw_BufferInfo";

        const string SHADER_BUFFER_INFO_VAL_PROPERTY = "_Wiwiw_BufferInfoVal";

        const string SHADER_BUFFER_INFO_INV_PROPERTY = "_Wiwiw_BufferInfoInv";

        const string SHADER_THREAD_INFO_PROPERTY = "_Wiwiw_ThreadInfo";

        static readonly Regex REGEX_THREAD_PER_WAVE_PATTERN = new
        (
            pattern: SHADER_THREADS_PER_WAVE_KEYWORD + @"_(\d+)",
            options: RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
        );

        // --------------------------------

        public static IEnumerator DispatchYield(ComputeShader computeShader, int3 bufferSize, int? sliceIndex = default, int? sliceCount = default)
        {
            if (!computeShader)
            {
                Debug.LogError($"({nameof(computeShader)}) is not valid");
                yield break;
            }

            if (math.any(bufferSize <= default(int)))
            {
                Debug.LogError($"({nameof(bufferSize)}) is not valid");
                yield break;
            }

            if (sliceIndex.HasValue && (sliceIndex < default(int) || sliceIndex >= bufferSize.z))
            {
                Debug.LogError($"({nameof(sliceIndex)}) is not valid");
                yield break;
            }

            if (sliceCount.HasValue && (sliceCount <= default(int) || sliceCount > bufferSize.z))
            {
                Debug.LogError($"({nameof(sliceCount)}) is not valid");
                yield break;
            }

            if (sliceIndex.HasValue && sliceCount.HasValue && sliceIndex.Value + sliceCount.Value > bufferSize.z)
            {
                Debug.LogError($"({nameof(sliceIndex)}) + ({nameof(sliceCount)}) is not valid");
                yield break;
            }

            var linearBufferSize = bufferSize.x * bufferSize.y * bufferSize.z;
            if (linearBufferSize <= default(int))
            {
                Debug.LogError($"({nameof(linearBufferSize)}) is not valid");
                yield break;
            }

            var kernelIndex = computeShader.FindKernel(SHADER_KERNEL_NAME);
            if (kernelIndex < default(int))
            {
                Debug.LogError($"No ({SHADER_KERNEL_NAME}) kernel found in ({nameof(computeShader)})");
                yield break;
            }

            var threadsPerWave = GetThreadsPerWave(computeShader);
            if (threadsPerWave <= default(int))
            {
                Debug.LogError($"({nameof(threadsPerWave)}) is not valid");
                yield break;
            }

            var threadsPerWaveTag = $"{SHADER_THREADS_PER_WAVE_KEYWORD}_{threadsPerWave}";
            if (string.IsNullOrEmpty(threadsPerWaveTag))
            {
                Debug.LogError($"({nameof(threadsPerWaveTag)}) is not valid");
                yield break;
            }

            var sliceSize = bufferSize.x * bufferSize.y;
            var startThreadOffset = sliceIndex.HasValue ? sliceIndex.Value * sliceSize : default;
            var finalThreadOffset = sliceCount.HasValue ? startThreadOffset + sliceCount.Value * sliceSize : linearBufferSize;

            computeShader.EnableKeyword(threadsPerWaveTag);

            for (var threadOffset = startThreadOffset; threadOffset < finalThreadOffset; threadOffset += THREADS_PER_DISPATCH)
            {
                var threadsCount = math.min(THREADS_PER_DISPATCH, finalThreadOffset - threadOffset);
                if (threadsCount <= default(int))
                {
                    Debug.LogError($"({nameof(threadsCount)}) is not valid");
                    yield break;
                }

                var groupsCount = (threadsCount + threadsPerWave - 1) / threadsPerWave;
                if (groupsCount <= default(int))
                {
                    Debug.LogError($"({nameof(groupsCount)}) is not valid");
                    yield break;
                }

                computeShader.SetInts(SHADER_BUFFER_INFO_PROPERTY, bufferSize.x, bufferSize.y, bufferSize.z, linearBufferSize);
                computeShader.SetFloats(SHADER_BUFFER_INFO_VAL_PROPERTY, bufferSize.x, bufferSize.y, bufferSize.z, linearBufferSize);
                computeShader.SetFloats(SHADER_BUFFER_INFO_INV_PROPERTY, math.rcp(bufferSize.x), math.rcp(bufferSize.y), math.rcp(bufferSize.z), math.rcp(linearBufferSize));
                computeShader.SetInts(SHADER_THREAD_INFO_PROPERTY, threadOffset, threadsCount, groupsCount, default);
                computeShader.Dispatch(kernelIndex, groupsCount, 1, 1);
                yield return default;
            }
        }

        // --------------------------------

        static int GetThreadsPerWave(ComputeShader computeShader)
        {
            if (!computeShader)
            {
                Debug.LogError($"({nameof(computeShader)}) is not valid");
                return default;
            }

            return computeShader.keywordSpace.keywordNames
                .Select(keyword => REGEX_THREAD_PER_WAVE_PATTERN.Match(keyword))
                .Where(match => match.Success)
                .Select(match => int.Parse(match.Groups[1].Value))
                .OrderByDescending(threads => threads)
                .FirstOrDefault(threads => threads <= SystemInfo.computeSubGroupSize);
        }

        // --------------------------------
    }
}
