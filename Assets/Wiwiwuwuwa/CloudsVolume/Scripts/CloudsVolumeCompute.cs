using System.Collections;
using UnityEngine;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeCompute : ComputeOperation
    {
        // ------------------------------------------------

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

            var cubemapOperation = new CloudsVolumeComputeCubemap(globalSettings, cubemapTexture);
            if (cubemapOperation is null)
            {
                Debug.LogError($"({nameof(cubemapOperation)}) is not valid");
                yield break;
            }

            while (cubemapOperation.MoveNext()) yield return default;

            var cookiesOperation = new CloudsVolumeComputeCookies(globalSettings, cookiesTexture);
            if (cookiesOperation is null)
            {
                Debug.LogError($"({nameof(cookiesOperation)}) is not valid");
                yield break;
            }

            while (cookiesOperation.MoveNext()) yield return default;
        }

        // ------------------------------------------------
    }
}
