using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCubemap : ComputeOperation
    {
        // ----------------------------------------------------

        const string SHADER_DENSITY_TEXTURE_PROPERTY = "_Wiwiw_DensityTexture";

        const string SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY = "_Wiwiw_DensityWorldToObjectMatrix";

        const string SHADER_CLOUDS_GRADIENT_PARAMS_PROPERTY = "_Wiwiw_CloudsGradientParams";

        const string SHADER_CLOUDS_GRADIENT_VALUES_PROPERTY = "_Wiwiw_CloudsGradientValues";

        const string SHADER_CLOUDS_CONTRAST_PROPERTY = "_Wiwiw_CloudsContrast";

        const string SHADER_CLOUDS_MIDPOINT_PROPERTY = "_Wiwiw_CloudsMidpoint";

        const string SHADER_SHADOWS_TEXTURE_PROPERTY = "_Wiwiw_ShadowsTexture";

        const string SHADER_SHADOWS_WORLD_TO_OBJECT_MATRIX_PROPERTY = "_Wiwiw_ShadowsWorldToObjectMatrix";

        const string SHADER_SHADOW_DIRECTION_PROPERTY = "_Wiwiw_ShadowDirection";

        const string SHADER_CUBEMAP_TEXTURE_PROPERTY = "_Wiwiw_CubemapTexture";

        const string SHADER_CUBEMAP_FACE_ID_PROPERTY = "_Wiwiw_CubemapFaceID";

        const string SHADER_CAMERA_POSITION_PROPERTY = "_Wiwiw_CameraPosition";

        const string SHADER_SKYBOX_SAMPLES_COUNT_VAL_PROPERTY = "_Wiwiw_SkyboxSamplesCountVal";

        const string SHADER_SKYBOX_SAMPLES_COUNT_RCP_PROPERTY = "_Wiwiw_SkyboxSamplesCountRcp";

        const string SHADER_SKYBOX_DENSITY_PROPERTY = "_Wiwiw_SkyboxDensity";

        // --------------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture densityTexture = default;

        readonly RenderTexture shadowsTexture = default;

        readonly RenderTexture cubemapTexture = default;

        // --------------------------------

        public CloudsVolumeComputeCubemap(CloudsVolumeGlobalSettings globalSettings, RenderTexture densityTexture, RenderTexture shadowsTexture, RenderTexture cubemapTexture)
        {
            this.globalSettings = globalSettings;
            this.densityTexture = densityTexture;
            this.shadowsTexture = shadowsTexture;
            this.cubemapTexture = cubemapTexture;
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

            if (!shadowsTexture)
            {
                Debug.LogError($"({nameof(shadowsTexture)}) is not valid");
                yield break;
            }

            if (!cubemapTexture)
            {
                Debug.LogError($"({nameof(cubemapTexture)}) is not valid");
                yield break;
            }

            var skyboxShader = globalSettings.SkyboxShader;
            if (!skyboxShader)
            {
                Debug.LogError($"({nameof(skyboxShader)}) is not valid");
                yield break;
            }

            skyboxShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);
            skyboxShader.SetMatrix(SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY, CloudsVolumeMatrices.GetDensityWorldToObject(globalSettings.CloudsAreaRange));
            skyboxShader.SetFloats(SHADER_CLOUDS_GRADIENT_PARAMS_PROPERTY, globalSettings.CloudsGradientParams);
            skyboxShader.SetFloats(SHADER_CLOUDS_GRADIENT_VALUES_PROPERTY, math.float4(0f, 1f, 1f, 0f));
            skyboxShader.SetFloat(SHADER_CLOUDS_CONTRAST_PROPERTY, globalSettings.CloudsContrast);
            skyboxShader.SetFloat(SHADER_CLOUDS_MIDPOINT_PROPERTY, globalSettings.CloudsMidpoint);
            skyboxShader.SetTexture(default, SHADER_SHADOWS_TEXTURE_PROPERTY, shadowsTexture);
            skyboxShader.SetMatrix(SHADER_SHADOWS_WORLD_TO_OBJECT_MATRIX_PROPERTY, CloudsVolumeMatrices.GetShadowsWorldToObjectMatrix(globalSettings.CloudsGradientParams.x, globalSettings.CloudsGradientParams.w, globalSettings.CloudsAreaRange));
            skyboxShader.SetVector(SHADER_SHADOW_DIRECTION_PROPERTY, CloudsVolumeEnvironment.GetSunForward());
            skyboxShader.SetVector(SHADER_CAMERA_POSITION_PROPERTY, CloudsVolumeEnvironment.GetEyePosition());
            skyboxShader.SetFloat(SHADER_SKYBOX_SAMPLES_COUNT_VAL_PROPERTY, globalSettings.SkyboxSamples);
            skyboxShader.SetFloat(SHADER_SKYBOX_SAMPLES_COUNT_RCP_PROPERTY, math.rcp(globalSettings.SkyboxSamples));
            skyboxShader.SetFloat(SHADER_SKYBOX_DENSITY_PROPERTY, globalSettings.SkyboxDensity);

            for (var cubemapFaceID = 0; cubemapFaceID < 6; cubemapFaceID++)
            {
                var cubemapFaceTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
                {
                    dimension = TextureDimension.Tex2D,
                    colorFormat = cubemapTexture.format,
                    width = cubemapTexture.width,
                    height = cubemapTexture.height,
                    volumeDepth = 1,
                    bindMS = false,
                    msaaSamples = 1,
                    enableRandomWrite = true,
                });

                if (!cubemapFaceTexture)
                {
                    Debug.LogError($"({nameof(cubemapFaceTexture)}) is not valid");
                    yield break;
                }

                Defer(() => RenderTexture.ReleaseTemporary(cubemapFaceTexture));
                skyboxShader.SetTexture(default, SHADER_CUBEMAP_TEXTURE_PROPERTY, cubemapFaceTexture);
                skyboxShader.SetFloat(SHADER_CUBEMAP_FACE_ID_PROPERTY, cubemapFaceID);

                var dispatchYield = WaveFrontUtils.DispatchYield
                (
                    computeShader: skyboxShader,
                    bufferSize: math.int3(cubemapTexture.width, cubemapTexture.height, cubemapTexture.volumeDepth)
                );
                while (dispatchYield.MoveNext()) yield return default;

                Graphics.CopyTexture
                (
                    src: cubemapFaceTexture,
                    srcElement: default,
                    srcMip: default,
                    dst: cubemapTexture,
                    dstElement: cubemapFaceID,
                    dstMip: default
                );

                RenderTexture.ReleaseTemporary(cubemapFaceTexture);
                yield return default;
            }
        }

        // ----------------------------------------------------
    }
}
