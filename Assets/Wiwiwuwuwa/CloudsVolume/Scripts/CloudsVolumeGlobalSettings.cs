using System;
using UnityEngine;
using Unity.Mathematics;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    [CreateAssetMenu(fileName = "CloudsVolumeGlobalSettings", menuName = "Wiwiwuwuwa/Clouds Volume/Global Settings")]
    public class CloudsVolumeGlobalSettings : ScriptableObject
    {
        // ------------------------------------------------

        [Header("Cubemap Settings")]

        [SerializeField]
        ComputeShader cubemapShader = default;

        [SerializeField]
        int cubemapTextureSize = 512;

        [Header("Cookies Settings")]

        [SerializeField]
        ComputeShader cookiesShader = default;

        [SerializeField]
        int cookiesTextureSize = 512;

        [Header("Density Texture Settings")]

        [SerializeField]
        Texture3D densityTexture = default;

        [SerializeField]
        float4 densityScale = math.float4(4096f, 512f, 128f, 32f);

        [SerializeField]
        float4 densityContrast = math.float4(0.25f, 0.5f, 0.5f, 0.5f);

        [SerializeField]
        float4 densityMidpoint = math.float4(0.75f, 0.25f, 0.25f, 0.25f);

        [Header("Gradient Texture Settings")]

        [SerializeField]
        float4 gradientPoints = math.float4(0f, 0.25f, 0.5f, 1f);

        [SerializeField]
        float4 gradientColors = math.float4(0f, 1f, 1f, 0f);

        [Header("Clouds Density Settings")]

        [SerializeField]
        float cloudsHeightMin = 128f;

        [SerializeField]
        float cloudsHeightMax = 192f;

        [Header("Bent Normals Settings")]

        [SerializeField]
        float bentNormalScale = 32f;

        [Header("Clouds Integration Settings")]

        [SerializeField]
        float cloudsSampleDensity = 0.05f;

        [SerializeField]
        float cloudsSampleDistance = 256f;

        [SerializeField]
        int cloudsSampleNumber = 32;

        // --------------------------------

        public event Action OnValidateEvent = default;

        public ComputeShader CubemapShader => cubemapShader;

        public int CubemapTextureSize => cubemapTextureSize;

        public ComputeShader CookiesShader => cookiesShader;

        public int CookiesTextureSize => cookiesTextureSize;

        public Texture3D DensityTexture => densityTexture;

        public float[] DensityMultiply => math.rcp(densityScale).ToArray();

        public float[] DensityContrast => math.float4(densityContrast).ToArray();

        public float[] DensityMidpoint => math.float4(densityMidpoint).ToArray();

        public float[] GradientPoints => math.float4(gradientPoints).ToArray();

        public float[] GradientColors => math.float4(gradientColors).ToArray();

        public float CloudsHeightMin => cloudsHeightMin;

        public float CloudsHeightMax => cloudsHeightMax;

        public float BentNormalScale => bentNormalScale;

        public float CloudsSampleStepDensity => cloudsSampleDensity * cloudsSampleDistance * math.rcp(cloudsSampleNumber);

        public float CloudsSampleFullDistance => cloudsSampleDistance;

        public float CloudsSampleNumberFlt => cloudsSampleNumber;

        public float CloudsSampleNumberRcp => math.rcp(cloudsSampleNumber);

        // --------------------------------

        void OnValidate()
        {
            OnValidateEvent?.Invoke();
        }

        // ------------------------------------------------
    }
}
