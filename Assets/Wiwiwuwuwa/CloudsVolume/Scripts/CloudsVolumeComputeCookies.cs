using System.Collections;
using UnityEngine;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCookies : ComputeOperation
    {
        // ------------------------------------------------

        const string SHADER_COOKIES_TEXTURE_PROPERTY = "_Wiwiw_CookiesTexture";

        // ----------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture cookiesTexture = default;

        // ----------------------------

        public CloudsVolumeComputeCookies(CloudsVolumeGlobalSettings globalSettings, RenderTexture cookiesTexture)
        {
            this.globalSettings = globalSettings;
            this.cookiesTexture = cookiesTexture;
        }

        protected override IEnumerator Execute()
        {
            if (!globalSettings)
            {
                Debug.LogError($"({nameof(globalSettings)}) is not valid");
                yield break;
            }

            if (!cookiesTexture)
            {
                Debug.LogError($"({nameof(cookiesTexture)}) is not valid");
                yield break;
            }

            var cookiesShader = globalSettings.CookiesShader;
            if (!cookiesShader)
            {
                Debug.LogError($"({nameof(cookiesShader)}) is not valid");
                yield break;
            }

            cookiesShader.SetTexture(default, SHADER_COOKIES_TEXTURE_PROPERTY, cookiesTexture);

            var dispatchOperation = new DispatchComputeShader(cookiesShader, cookiesTexture.GetSize());
            if (dispatchOperation is null)
            {
                Debug.LogError($"({nameof(dispatchOperation)}) is not valid");
                yield break;
            }

            while (dispatchOperation.MoveNext()) yield return default;
        }

        // ------------------------------------------------
    }
}
