using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeDensity : ComputeOperation
    {
        // ----------------------------------------------------

        const string SHADER_DENSITY_TEXTURE_PROPERTY = "_Wiwiw_DensityTexture";

        const string SHADER_DENSITY_OBJECT_TO_WORLD_MATRIX_PROPERTY = "_Wiwiw_DensityObjectToWorldMatrix";

        const string SHADER_DENSITY_PARAMS_PROPERTY = "_Wiwiw_DensityParams";

        const string SHADER_GRADIENT_PARAMS_PROPERTY = "_Wiwiw_GradientParams";

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture densityTexture = default;

        // --------------------------------

        public CloudsVolumeComputeDensity(CloudsVolumeGlobalSettings globalSettings, RenderTexture densityTexture)
        {
            this.globalSettings = globalSettings;
            this.densityTexture = densityTexture;
        }

        protected override IEnumerator Execute()
        {
            if (!globalSettings)
            {
                Debug.LogError($"({nameof(globalSettings)}) is not valid");
                yield break;
            }

            if (!densityTexture)
            {
                Debug.LogError($"({nameof(densityTexture)}) is not valid");
                yield break;
            }

            var densityComputeShader = globalSettings.DensityComputeShader;
            if (!densityComputeShader)
            {
                Debug.LogError($"({nameof(densityComputeShader)}) is not valid");
                yield break;
            }

            densityComputeShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);
            densityComputeShader.SetMatrix(SHADER_DENSITY_OBJECT_TO_WORLD_MATRIX_PROPERTY, ClodusVolumeMatrices.GetDensityObjectToWorldMatrix());
            densityComputeShader.SetVector(SHADER_DENSITY_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.DensityNoiseScale,
                y: globalSettings.DensityContrast,
                z: globalSettings.DensityMidpoint,
                w: default
            ));
            densityComputeShader.SetVector(SHADER_GRADIENT_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.DensityFadeInStartPos,
                y: globalSettings.DensityFadeInFinalPos,
                z: globalSettings.DensityFadeOutStartPos,
                w: globalSettings.DensityFadeOutFinalPos
            ));

            var dispatchYield = WaveFrontUtils.DispatchYield
            (
                computeShader: densityComputeShader,
                bufferSize: math.int3(densityTexture.width, densityTexture.height, densityTexture.volumeDepth)
            );
            while (dispatchYield.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
