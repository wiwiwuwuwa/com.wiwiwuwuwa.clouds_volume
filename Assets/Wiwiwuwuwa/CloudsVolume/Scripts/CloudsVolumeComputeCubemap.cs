using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCubemap : ComputeOperation
    {
        // ------------------------------------------------

        const string SHADER_CUBEMAP_FACE_TEXTURE_PROPERTY = "_Wiwiw_CubemapFaceTexture";

        const string SHADER_CUBEMAP_FACE_INDEX_PROPERTY = "_Wiwiw_CubemapFaceIndex";

        // ----------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture cubemapTexture = default;

        // ----------------------------

        public CloudsVolumeComputeCubemap(CloudsVolumeGlobalSettings globalSettings, RenderTexture cubemapTexture)
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

            var cubemapShader = globalSettings.CubemapShader;
            if (!cubemapShader)
            {
                Debug.LogError($"({nameof(cubemapShader)}) is not valid");
                yield break;
            }

            for (var cubemapFaceIndex = 0; cubemapFaceIndex < 6; cubemapFaceIndex++)
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

                cubemapShader.SetTexture(default, SHADER_CUBEMAP_FACE_TEXTURE_PROPERTY, cubemapFaceTexture);
                cubemapShader.SetInt(SHADER_CUBEMAP_FACE_INDEX_PROPERTY, cubemapFaceIndex);

                var dispatchOperation = new DispatchComputeShader(cubemapShader, cubemapTexture.GetSize());
                if (dispatchOperation is null)
                {
                    Debug.LogError($"({nameof(dispatchOperation)}) is not valid");
                    yield break;
                }

                while (dispatchOperation.MoveNext()) yield return default;

                Graphics.CopyTexture
                (
                    src: cubemapFaceTexture,
                    srcElement: default,
                    srcMip: default,
                    dst: cubemapTexture,
                    dstElement: cubemapFaceIndex,
                    dstMip: default
                );

                RenderTexture.ReleaseTemporary(cubemapFaceTexture);
                yield return default;
            }
        }

        // ------------------------------------------------
    }
}
