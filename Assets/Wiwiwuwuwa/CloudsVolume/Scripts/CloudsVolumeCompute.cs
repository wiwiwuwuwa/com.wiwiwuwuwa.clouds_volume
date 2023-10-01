using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeCompute : ComputeOperation
    {
        // ----------------------------------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture cubemapTexture = default;

        // ----------------------------

        public CloudsVolumeCompute(CloudsVolumeGlobalSettings globalSettings, RenderTexture cubemapTexture)
        {
            this.globalSettings = globalSettings;
            this.cubemapTexture = cubemapTexture;
        }

        protected override IEnumerator Execute()
        {
            if (!globalSettings)
            {
                Debug.LogError($"({nameof(globalSettings)}) is not valid");
                yield break;
            }

            if (!cubemapTexture)
            {
                Debug.LogError($"({nameof(cubemapTexture)}) is not valid");
                yield break;
            }

            var densityTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex3D,
                colorFormat = RenderTextureFormat.R8,
                width = globalSettings.DensityTextureSize,
                height = globalSettings.DensityTextureSize >> 1,
                volumeDepth = globalSettings.DensityTextureSize,
                bindMS = false,
                msaaSamples = 1,
                enableRandomWrite = true,
            });

            if (!densityTexture)
            {
                Debug.LogError($"({nameof(densityTexture)}) is not valid");
                yield break;
            }

            Defer(() => RenderTexture.ReleaseTemporary(densityTexture));
            densityTexture.filterMode = FilterMode.Bilinear;
            densityTexture.wrapModeU = TextureWrapMode.Repeat;
            densityTexture.wrapModeV = TextureWrapMode.Clamp;
            densityTexture.wrapModeW = TextureWrapMode.Repeat;

            var shadowsTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex3D,
                colorFormat = RenderTextureFormat.R8,
                width = globalSettings.ShadowsTextureSize,
                height = globalSettings.ShadowsTextureSize,
                volumeDepth = globalSettings.ShadowsTextureSize,
                bindMS = false,
                msaaSamples = 1,
                enableRandomWrite = true,
            });

            if (!shadowsTexture)
            {
                Debug.LogError($"({nameof(shadowsTexture)}) is not valid");
                yield break;
            }

            Defer(() => RenderTexture.ReleaseTemporary(shadowsTexture));
            shadowsTexture.filterMode = FilterMode.Bilinear;
            shadowsTexture.wrapMode = TextureWrapMode.Clamp;

            var densityCompute = new CloudsVolumeComputeDensity(globalSettings, densityTexture);
            if (densityCompute is null)
            {
                Debug.LogError($"({nameof(densityCompute)}) is not valid");
                yield break;
            }

            Defer(() => densityCompute.Dispose());
            while (densityCompute.MoveNext()) yield return default;

            var shadowsCompute = new CloudsVolumeComputeShadows(globalSettings, densityTexture, shadowsTexture);
            if (shadowsCompute is null)
            {
                Debug.LogError($"({nameof(shadowsCompute)}) is not valid");
                yield break;
            }

            Defer(() => shadowsCompute.Dispose());
            while (shadowsCompute.MoveNext()) yield return default;

            var cubemapCompute = new CloudsVolumeComputeCubemap(globalSettings, densityTexture, shadowsTexture, cubemapTexture);
            if (cubemapCompute is null)
            {
                Debug.LogError($"({nameof(cubemapCompute)}) is not valid");
                yield break;
            }

            Defer(() => cubemapCompute.Dispose());
            while (cubemapCompute.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
