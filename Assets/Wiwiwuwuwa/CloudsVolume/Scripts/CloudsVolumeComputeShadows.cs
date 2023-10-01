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

        const string SHADER_SHADOWS_TEXTURE_PROPERTY = "_Wiwiw_ShadowsTexture";

        const string SHADER_SHADOWS_OBJECT_TO_WORLD_MATRIX_PROPERTY = "_Wiwiw_ShadowsObjectToWorldMatrix";

        const string SHADER_DENSITY_PARAMS_PROPERTY = "_Wiwiw_DensityParams";

        const string SHADER_SHADOWS_PARAMS_PROPERTY = "_Wiwiw_ShadowsParams";

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

            var shadowsComputeShader = globalSettings.ShadowsComputeShader;
            if (!shadowsComputeShader)
            {
                Debug.LogError($"({nameof(shadowsComputeShader)}) is not valid");
                yield break;
            }

            shadowsComputeShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);
            shadowsComputeShader.SetMatrix(SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY, ClodusVolumeMatrices.GetDensityWorldToObjectMatrix());
            shadowsComputeShader.SetTexture(default, SHADER_SHADOWS_TEXTURE_PROPERTY, shadowsTexture);
            shadowsComputeShader.SetMatrix(SHADER_SHADOWS_OBJECT_TO_WORLD_MATRIX_PROPERTY, ClodusVolumeMatrices.GetShadowsObjectToWorldMatrix(globalSettings.ShadowsAreaScale));
            shadowsComputeShader.SetVector(SHADER_DENSITY_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.DensityFadeInStartPos,
                y: globalSettings.DensityFadeOutFinalPos,
                z: default,
                w: default
            ));
            shadowsComputeShader.SetVector(SHADER_SHADOWS_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.ShadowsSamples,
                y: math.rcp(globalSettings.ShadowsSamples),
                z: globalSettings.ShadowsDensity,
                w: default
            ));

            var dispatchYield = WaveFrontUtils.DispatchYield
            (
                computeShader: shadowsComputeShader,
                bufferSize: math.int3(shadowsTexture.width, shadowsTexture.height, shadowsTexture.volumeDepth)
            );
            while (dispatchYield.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
