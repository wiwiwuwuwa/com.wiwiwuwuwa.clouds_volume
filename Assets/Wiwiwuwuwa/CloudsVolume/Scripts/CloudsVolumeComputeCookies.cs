using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCookies : ComputeOperation
    {
        // ----------------------------------------------------

        const string SHADER_SHADOWS_TEXTURE_PROPERTY = "_Wiwiw_ShadowsTexture";

        const string SHADER_COOKIES_TEXTURE_PROPERTY = "_Wiwiw_CookiesTexture";

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture shadowsTexture = default;

        readonly RenderTexture cookiesTexture = default;

        // --------------------------------

        public CloudsVolumeComputeCookies(CloudsVolumeGlobalSettings globalSettings, RenderTexture shadowsTexture, RenderTexture cookiesTexture)
        {
            this.globalSettings = globalSettings;
            this.shadowsTexture = shadowsTexture;
            this.cookiesTexture = cookiesTexture;
        }

        // --------------------------------

        protected override IEnumerator Execute()
        {
            if (!shadowsTexture)
            {
                Debug.LogError($"({nameof(shadowsTexture)}) is not valid");
                yield break;
            }

            if (!cookiesTexture)
            {
                Debug.LogError($"({nameof(cookiesTexture)}) is not valid");
                yield break;
            }

            var cookiesComputeShader = globalSettings.CookiesComputeShader;
            if (!cookiesComputeShader)
            {
                Debug.LogError($"({nameof(cookiesComputeShader)}) is not valid");
                yield break;
            }

            cookiesComputeShader.SetTexture(default, SHADER_SHADOWS_TEXTURE_PROPERTY, shadowsTexture);
            cookiesComputeShader.SetTexture(default, SHADER_COOKIES_TEXTURE_PROPERTY, cookiesTexture);

            var dispatchYield = WaveFrontUtils.DispatchYield
            (
                computeShader: cookiesComputeShader,
                bufferSize: math.int3(cookiesTexture.width, cookiesTexture.height, cookiesTexture.volumeDepth)
            );
            while (dispatchYield.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
