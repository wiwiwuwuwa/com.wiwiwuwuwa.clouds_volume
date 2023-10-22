using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Mathematics;

namespace Wiwiwuwuwa.Utilities
{
    public class DispatchComputeShader : ComputeOperation
    {
        // ------------------------------------------------

        const int DEFAULT_DISPATCH_SIZE = 128 * 128;

        const string SHADER_KERNEL_NAME = "CSMain";

        const string SHADER_WAVEFRONT_SIZE_KEYWORD = "WIWIW_WAVEFRONT_SIZE";

        const string SHADER_BUFFER_SIZE_INT3_PROPERTY = "_Wiwiw_BufferSizeInt3";

        const string SHADER_BUFFER_SIZE_FLT3_PROPERTY = "_Wiwiw_BufferSizeFlt3";

        const string SHADER_BUFFER_SIZE_RCP3_PROPERTY = "_Wiwiw_BufferSizeRcp3";

        const string SHADER_THREAD_OFFSET_PROPERTY = "_Wiwiw_ThreadOffset";

        // ----------------------------

        static readonly Regex REGEX_WAVEFRONT_SIZE_PATTERN = new
        (
            pattern: SHADER_WAVEFRONT_SIZE_KEYWORD + @"_(\d+)",
            options: RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
        );

        // ----------------------------

        readonly ComputeShader computeShader = default;

        readonly int3 bufferSize = default;

        readonly int? dispatchSize = default;

        readonly int? sliceIndex = default;

        readonly int? sliceCount = default;

        // ----------------------------

        public DispatchComputeShader
        (
            ComputeShader computeShader,
            int3 bufferSize,
            int? dispatchSize = default,
            int? sliceIndex = default,
            int? sliceCount = default
        )
        {
            this.computeShader = computeShader;
            this.bufferSize = bufferSize;
            this.dispatchSize = dispatchSize;
            this.sliceIndex = sliceIndex;
            this.sliceCount = sliceCount;
        }

        protected override IEnumerator Execute()
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

            if (dispatchSize.HasValue && dispatchSize <= default(int))
            {
                Debug.LogError($"({nameof(dispatchSize)}) is not valid");
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

            var kernelIndex = computeShader.FindKernel(SHADER_KERNEL_NAME);
            if (kernelIndex < default(int))
            {
                Debug.LogError($"({nameof(kernelIndex)}) is not valid");
                yield break;
            }

            var wavefrontSize = GetWavefrontSize(computeShader);
            if (wavefrontSize <= default(int))
            {
                Debug.LogError($"({nameof(wavefrontSize)}) is not valid");
                yield break;
            }

            var wavefrontKeyword = $"{SHADER_WAVEFRONT_SIZE_KEYWORD}_{wavefrontSize}";
            if (string.IsNullOrEmpty(wavefrontKeyword))
            {
                Debug.LogError($"({nameof(wavefrontKeyword)}) is not valid");
                yield break;
            }

            computeShader.EnableKeyword(wavefrontKeyword);

            var linearDispatchSize = dispatchSize ?? DEFAULT_DISPATCH_SIZE;
            var linearBufferSize = bufferSize.x * bufferSize.y * bufferSize.z;
            var linearSliceSize = bufferSize.x * bufferSize.y;
            var threadIndexStart = sliceIndex.HasValue ? sliceIndex.Value * linearSliceSize : default;
            var threadIndexFinal = sliceCount.HasValue ? threadIndexStart + sliceCount.Value * linearSliceSize : linearBufferSize;

            for (var threadOffset = threadIndexStart; threadOffset < threadIndexFinal; threadOffset += linearDispatchSize)
            {
                var threadCount = math.min(linearDispatchSize, threadIndexFinal - threadOffset);
                if (threadCount <= default(int))
                {
                    Debug.LogError($"({nameof(threadCount)}) is not valid");
                    yield break;
                }

                var groupsCount = (threadCount + wavefrontSize - 1) / wavefrontSize;
                if (groupsCount <= default(int))
                {
                    Debug.LogError($"({nameof(groupsCount)}) is not valid");
                    yield break;
                }

                computeShader.SetInts(SHADER_BUFFER_SIZE_INT3_PROPERTY, bufferSize.x, bufferSize.y, bufferSize.z);
                computeShader.SetFloats(SHADER_BUFFER_SIZE_FLT3_PROPERTY, bufferSize.x, bufferSize.y, bufferSize.z);
                computeShader.SetFloats(SHADER_BUFFER_SIZE_RCP3_PROPERTY, math.rcp(bufferSize.x), math.rcp(bufferSize.y), math.rcp(bufferSize.z));
                computeShader.SetInt(SHADER_THREAD_OFFSET_PROPERTY, threadOffset);
                computeShader.Dispatch(kernelIndex, groupsCount, 1, 1);
                yield return default;
            }
        }

        // ----------------------------

        static int GetWavefrontSize(ComputeShader computeShader)
        {
            if (!computeShader)
            {
                Debug.LogError($"({nameof(computeShader)}) is not valid");
                return default;
            }

            return computeShader.keywordSpace.keywordNames
                .Select(keyword => REGEX_WAVEFRONT_SIZE_PATTERN.Match(keyword))
                .Where(match => match.Success)
                .Select(match => int.Parse(match.Groups[1].Value))
                .OrderByDescending(threads => threads)
                .FirstOrDefault(threads => threads <= SystemInfo.computeSubGroupSize);
        }

        // ------------------------------------------------
    }
}
