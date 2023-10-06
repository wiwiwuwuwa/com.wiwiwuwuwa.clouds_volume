using System;
using UnityEngine;

namespace Wiwiwuwuwa.CloudsVolume
{
    [CreateAssetMenu(fileName = "CloudsVolumeGlobalSettings", menuName = "Wiwiwuwuwa/CloudsVolume/Global Settings")]
    public class CloudsVolumeGlobalSettings : ScriptableObject
    {
        // ----------------------------------------------------

        [SerializeField]
        ComputeShader densityComputeShader = default;

        [SerializeField]
        int densityTextureSize = 64;

        [SerializeField]
        float densityNoiseScale = 6f;

        [SerializeField]
        float densityContrast = 0.85f;

        [SerializeField]
        float densityMidpoint = 0.35f;

        [SerializeField]
        float densityFadeInStartPos = 0.40f;

        [SerializeField]
        float densityFadeInFinalPos = 0.45f;

        [SerializeField]
        float densityFadeOutStartPos = 0.55f;

        [SerializeField]
        float densityFadeOutFinalPos = 0.60f;

        [SerializeField]
        ComputeShader shadowsComputeShader = default;

        [SerializeField]
        int shadowsTextureSize = 128;

        [SerializeField]
        float shadowsAreaScale = 2f;

        [SerializeField]
        int shadowsSamples = 32;

        [SerializeField]
        float shadowsDensity = 1f;

        [SerializeField]
        ComputeShader cubemapComputeShader = default;

        [SerializeField]
        int cubemapTextureSize = 512;

        [SerializeField]
        int cubemapSamples = 32;

        [SerializeField]
        float cubemapDensity = 0.5f;

        [SerializeField]
        ComputeShader cookiesComputeShader = default;

        [SerializeField]
        int cookiesTextureSize = 128;

        // --------------------------------

        public event Action OnValidateEvent;

        // --------------------------------

        public ComputeShader DensityComputeShader => densityComputeShader;

        public int DensityTextureSize => densityTextureSize;

        public float DensityNoiseScale => densityNoiseScale;

        public float DensityContrast => densityContrast;

        public float DensityMidpoint => densityMidpoint;

        public float DensityFadeInStartPos => densityFadeInStartPos;

        public float DensityFadeInFinalPos => densityFadeInFinalPos;

        public float DensityFadeOutStartPos => densityFadeOutStartPos;

        public float DensityFadeOutFinalPos => densityFadeOutFinalPos;

        public ComputeShader ShadowsComputeShader => shadowsComputeShader;

        public int ShadowsTextureSize => shadowsTextureSize;

        public float ShadowsAreaScale => shadowsAreaScale;

        public float ShadowsSamples => shadowsSamples;

        public float ShadowsDensity => shadowsDensity;

        public ComputeShader CubemapComputeShader => cubemapComputeShader;

        public int CubemapTextureSize => cubemapTextureSize;

        public float CubemapSamples => cubemapSamples;

        public float CubemapDensity => cubemapDensity;

        public ComputeShader CookiesComputeShader => cookiesComputeShader;

        public int CookiesTextureSize => cookiesTextureSize;

        // --------------------------------

        void OnValidate()
        {
            OnValidateEvent?.Invoke();
        }

        // ----------------------------------------------------
    }
}
