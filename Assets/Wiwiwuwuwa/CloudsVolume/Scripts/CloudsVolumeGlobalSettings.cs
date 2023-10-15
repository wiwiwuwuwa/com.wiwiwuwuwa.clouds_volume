using System;
using Unity.Mathematics;
using UnityEngine;

namespace Wiwiwuwuwa.CloudsVolume
{
    [CreateAssetMenu(fileName = "CloudsVolumeGlobalSettings", menuName = "Wiwiwuwuwa/CloudsVolume/Global Settings")]
    public class CloudsVolumeGlobalSettings : ScriptableObject
    {
        // ----------------------------------------------------

        [SerializeField]
        ComputeShader cloudsShader = default;

        [SerializeField]
        int cloudsTextureSize = 64;

        [SerializeField]
        float cloudsAreaRange = 256f;

        [SerializeField]
        float4 cloudsGradientParams = math.float4(128f, 160f, 160f, 192f);

        [SerializeField]
        float cloudsContrast = 0.5f;

        [SerializeField]
        float cloudsMidpoint = 0.2f;

        [SerializeField]
        ComputeShader shadowShader = default;

        [SerializeField]
        int shadowTextureSize = 128;

        [SerializeField]
        int shadowSamples = 32;

        [SerializeField]
        float shadowDensity = 1f;

        [SerializeField]
        ComputeShader cubemapShader = default;

        [SerializeField]
        int cubemapTextureSize = 512;

        [SerializeField]
        int cubemapSamples = 32;

        [SerializeField]
        float cubemapDensity = 1f;

        [SerializeField]
        ComputeShader cookiesShader = default;

        [SerializeField]
        int cookiesTextureSize = 128;

        // --------------------------------

        public event Action OnValidateEvent;

        // --------------------------------

        public ComputeShader CloudsShader => cloudsShader;

        public int CloudsTextureSize => cloudsTextureSize;

        public float CloudsAreaRange => cloudsAreaRange;

        public float4 CloudsGradientParams => cloudsGradientParams;

        public float CloudsContrast => cloudsContrast;

        public float CloudsMidpoint => math.saturate(1f - cloudsMidpoint);

        public ComputeShader ShadowShader => shadowShader;

        public int ShadowTextureSize => shadowTextureSize;

        public int ShadowSamples => shadowSamples;

        public float ShadowDensity => shadowDensity;

        public ComputeShader CubemapShader => cubemapShader;

        public int CubemapTextureSize => cubemapTextureSize;

        public float CubemapSamples => cubemapSamples;

        public float CubemapDensity => cubemapDensity;

        public ComputeShader CookiesShader => cookiesShader;

        public int CookiesTextureSize => cookiesTextureSize;

        // --------------------------------

        void OnValidate()
        {
            OnValidateEvent?.Invoke();
        }

        // ----------------------------------------------------
    }
}
