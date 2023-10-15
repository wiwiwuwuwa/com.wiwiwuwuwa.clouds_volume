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

        readonly RenderTexture cookiesTexture = default;

        // ----------------------------

        public CloudsVolumeCompute(CloudsVolumeGlobalSettings globalSettings, RenderTexture cubemapTexture, RenderTexture cookiesTexture)
        {
            this.globalSettings = globalSettings;
            this.cubemapTexture = cubemapTexture;
            this.cookiesTexture = cookiesTexture;
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

            if (!cookiesTexture)
            {
                Debug.LogError($"({nameof(cookiesTexture)}) is not valid");
                yield break;
            }

            var densityTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex3D,
                colorFormat = RenderTextureFormat.R8,
                width = globalSettings.CloudsTextureSize,
                height = globalSettings.CloudsTextureSize,
                volumeDepth = globalSettings.CloudsTextureSize,
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
            densityTexture.wrapMode = TextureWrapMode.Repeat;

            var shadowsTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex3D,
                colorFormat = RenderTextureFormat.R8,
                width = globalSettings.ShadowTextureSize,
                height = globalSettings.ShadowTextureSize,
                volumeDepth = globalSettings.ShadowTextureSize,
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
            shadowsTexture.wrapModeU = TextureWrapMode.Repeat;
            shadowsTexture.wrapModeV = TextureWrapMode.Clamp;
            shadowsTexture.wrapModeW = TextureWrapMode.Repeat;

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

            var cookiesCompute = new CloudsVolumeComputeCookies(globalSettings, shadowsTexture, cookiesTexture);
            if (cookiesCompute is null)
            {
                Debug.LogError($"({nameof(cookiesCompute)}) is not valid");
                yield break;
            }

            Defer(() => cookiesCompute.Dispose());
            while (cookiesCompute.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
