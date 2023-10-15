using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeDensity : ComputeOperation
    {
        // ----------------------------------------------------

        const string SHADER_DENSITY_TEXTURE_PROPERTY = "_Wiwiw_DensityTexture";

        // --------------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture densityTexture = default;

        // --------------------------------

        public CloudsVolumeComputeDensity(CloudsVolumeGlobalSettings globalSettings, RenderTexture densityTexture)
        {
            this.globalSettings = globalSettings;
            this.densityTexture = densityTexture;
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

            var cloudsShader = globalSettings.CloudsShader;
            if (!cloudsShader)
            {
                Debug.LogError($"({nameof(cloudsShader)}) is not valid");
                yield break;
            }

            cloudsShader.SetTexture(default, SHADER_DENSITY_TEXTURE_PROPERTY, densityTexture);

            var dispatchYield = WaveFrontUtils.DispatchYield
            (
                computeShader: cloudsShader,
                bufferSize: math.int3(densityTexture.width, densityTexture.height, densityTexture.volumeDepth)
            );
            while (dispatchYield.MoveNext()) yield return default;
        }

        // ----------------------------------------------------
    }
}
