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

        const string SHADER_SHADOWS_TEXTURE_PROPERTY = "_Wiwiw_ShadowsTexture";

        const string SHADER_SHADOWS_WORLD_TO_OBJECT_MATRIX_PROPERTY = "_Wiwiw_ShadowsWorldToObjectMatrix";

        const string SHADER_DENSITY_PARAMS_PROPERTY = "_Wiwiw_DensityParams";

        const string SHADER_CUBEMAP_PARAMS_PROPERTY = "_Wiwiw_CubemapParams";

        const string SHADER_CUBEMAP_TEXTURE_PROPERTY = "_Wiwiw_CubemapTexture";

        const string SHADER_CUBEMAP_FACE_ID_PROPERTY = "_Wiwiw_CubemapFaceID";

        const string SHADER_SUN_DIR_PROPERTY = "_Wiwiw_SunDir";

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

            var cubemapComputeShader = globalSettings.CubemapComputeShader;
            if (!cubemapComputeShader)
            {
                Debug.LogError($"({nameof(cubemapComputeShader)}) is not valid");
                yield break;
            }

            cubemapComputeShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);
            cubemapComputeShader.SetMatrix(SHADER_DENSITY_WORLD_TO_OBJECT_MATRIX_PROPERTY, ClodusVolumeMatrices.GetDensityWorldToObjectMatrix());
            cubemapComputeShader.SetTexture(default, SHADER_SHADOWS_TEXTURE_PROPERTY, shadowsTexture);
            cubemapComputeShader.SetMatrix(SHADER_SHADOWS_WORLD_TO_OBJECT_MATRIX_PROPERTY, ClodusVolumeMatrices.GetShadowsWorldToObjectMatrix(globalSettings.ShadowsAreaScale));
            cubemapComputeShader.SetVector(SHADER_DENSITY_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.DensityFadeInStartPos,
                y: globalSettings.DensityFadeOutFinalPos,
                z: default,
                w: default
            ));
            cubemapComputeShader.SetVector(SHADER_CUBEMAP_PARAMS_PROPERTY, math.float4
            (
                x: globalSettings.CubemapSamples,
                y: math.rcp(globalSettings.CubemapSamples),
                z: globalSettings.CubemapDensity,
                w: default
            ));
            cubemapComputeShader.SetVector(SHADER_SUN_DIR_PROPERTY, math.float4(CloudsVolumeEnvironment.GetSunForwardVector(), 0f));

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
                cubemapComputeShader.SetTexture(default, SHADER_CUBEMAP_TEXTURE_PROPERTY, cubemapFaceTexture);
                cubemapComputeShader.SetInt(SHADER_CUBEMAP_FACE_ID_PROPERTY, cubemapFaceID);

                var dispatchYield = WaveFrontUtils.DispatchYield
                (
                    computeShader: cubemapComputeShader,
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
