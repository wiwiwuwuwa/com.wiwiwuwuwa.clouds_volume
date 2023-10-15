using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeShadows : ComputeOperation
    {
        // ----------------------------------------------------

        const string SHADER_DENSITY_TEXTURE_PROPERTY = "_Wiwiw_DensityTexture";

        const string SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY = "_Wiwiw_DensityWorldToObjectMatrix";

        const string SHADER_CLOUDS_GRADIENT_PARAMS_PROPERTY = "_Wiwiw_CloudsGradientParams";

        const string SHADER_CLOUDS_GRADIENT_VALUES_PROPERTY = "_Wiwiw_CloudsGradientValues";

        const string SHADER_CLOUDS_CONTRAST_PROPERTY = "_Wiwiw_CloudsContrast";

        const string SHADER_CLOUDS_MIDPOINT_PROPERTY = "_Wiwiw_CloudsMidpoint";

        const string SHADER_SHADOWS_TEXTURE_PROPERTY = "_Wiwiw_ShadowsTexture";

        const string SHADER_SHADOWS_OBJECT_TO_WORLD_MATRIX_PROPERTY = "_Wiwiw_ShadowsObjectToWorldMatrix";

        const string SHADER_SHADOW_DIRECTION_PROPERTY = "_Wiwiw_ShadowDirection";

        const string SHADER_SHADOW_SAMPLES_COUNT_VAL_PROPERTY = "_Wiwiw_ShadowSamplesCountVal";

        const string SHADER_SHADOW_SAMPLES_COUNT_RCP_PROPERTY = "_Wiwiw_ShadowSamplesCountRcp";

        const string SHADER_SHADOW_DENSITY_PROPERTY = "_Wiwiw_ShadowDensity";

        // --------------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture densityTexture = default;

        readonly RenderTexture shadowsTexture = default;

        // --------------------------------

        public CloudsVolumeComputeShadows(CloudsVolumeGlobalSettings globalSettings, RenderTexture densityTexture, RenderTexture shadowsTexture)
        {
            this.globalSettings = globalSettings;
            this.densityTexture = densityTexture;
            this.shadowsTexture = shadowsTexture;
        }

        // --------------------------------

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

            if (!shadowsTexture)
            {
                Debug.LogError($"({nameof(shadowsTexture)}) is not valid");
                yield break;
            }

            var shadowShader = globalSettings.ShadowShader;
            if (!shadowShader)
            {
                Debug.LogError($"({nameof(shadowShader)}) is not valid");
                yield break;
            }

            shadowShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);
            shadowShader.SetMatrix(SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY, CloudsVolumeMatrices.GetDensityWorldToObject(globalSettings.CloudsAreaRange));
            shadowShader.SetFloats(SHADER_CLOUDS_GRADIENT_PARAMS_PROPERTY, globalSettings.CloudsGradientParams);
            shadowShader.SetFloats(SHADER_CLOUDS_GRADIENT_VALUES_PROPERTY, math.float4(0f, 1f, 1f, 0f));
            shadowShader.SetFloat(SHADER_CLOUDS_CONTRAST_PROPERTY, globalSettings.CloudsContrast);
            shadowShader.SetFloat(SHADER_CLOUDS_MIDPOINT_PROPERTY, globalSettings.CloudsMidpoint);
            shadowShader.SetTexture(default, SHADER_SHADOWS_TEXTURE_PROPERTY, shadowsTexture);
            shadowShader.SetMatrix(SHADER_SHADOWS_OBJECT_TO_WORLD_MATRIX_PROPERTY, CloudsVolumeMatrices.GetShadowsObjectToWorldMatrix(globalSettings.CloudsGradientParams.x, globalSettings.CloudsGradientParams.w, globalSettings.CloudsAreaRange));
            shadowShader.SetVector(SHADER_SHADOW_DIRECTION_PROPERTY, CloudsVolumeEnvironment.GetSunForward());
            shadowShader.SetFloat(SHADER_SHADOW_SAMPLES_COUNT_VAL_PROPERTY, globalSettings.ShadowSamples);
            shadowShader.SetFloat(SHADER_SHADOW_SAMPLES_COUNT_RCP_PROPERTY, math.rcp(globalSettings.ShadowSamples));
            shadowShader.SetFloat(SHADER_SHADOW_DENSITY_PROPERTY, globalSettings.ShadowDensity);

            var dispatchYield = WaveFrontUtils.DispatchYield
            (
                computeShader: shadowShader,
                bufferSize: math.int3(shadowsTexture.width, shadowsTexture.height, shadowsTexture.volumeDepth)
            );
            while (dispatchYield.MoveNext()) yield return default;
        }
    }
}
