using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    [ExecuteInEditMode]
    public class CloudsVolumeRenderer : MonoBehaviour
    {
        // ------------------------------------------------

        [Header("Global Settings")]

        [SerializeField]
        CloudsVolumeGlobalSettings globalSettings = default;

        [NonSerialized]
        CloudsVolumeGlobalSettings cachedSettings = default;

        // ----------------------------

        void OnValidate()
        {
            OnEnable();
        }

        void OnEnable()
        {
            OnDisable();

            InitSystems();

            if (globalSettings)
            {
                cachedSettings = globalSettings;
                cachedSettings.OnValidateEvent += OnValidate;
            }
        }

        void Update()
        {
            TickSystems();
        }

        void OnDisable()
        {
            if (cachedSettings)
            {
                cachedSettings.OnValidateEvent -= OnValidate;
                cachedSettings = default;
            }

            ShutSystems();
        }

        // ------------------------------------------------

        void InitSystems()
        {
            ShutSystems();

            InitCubemapTexture();
            InitCookiesTexture();
            InitComputeOperation();
            SyncRendererProperties();
        }

        void TickSystems()
        {
            TickComputeOperation();
            TickReflectionProbe();
            SyncRendererProperties();
        }

        void ShutSystems()
        {
            SyncRendererProperties();
            ShutComputeOperation();
            ShutCookiesTexture();
            ShutCubemapTexture();
        }

        // ------------------------------------------------

        RenderTexture cubemapTexture = default;

        // ----------------------------

        void InitCubemapTexture()
        {
            ShutCubemapTexture();

            if (!globalSettings)
            {
                return;
            }

            cubemapTexture = new RenderTexture(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex2DArray,
                colorFormat = RenderTextureFormat.ARGB32,
                width = globalSettings.CubemapTextureSize,
                height = globalSettings.CubemapTextureSize,
                volumeDepth = 6,
                bindMS = false,
                msaaSamples = 1,
                enableRandomWrite = true,
            })
            {
                name = "Clouds Volume Cubemap Texture",
            };

            if (!cubemapTexture)
            {
                return;
            }

            cubemapTexture.filterMode = FilterMode.Bilinear;
            cubemapTexture.wrapMode = TextureWrapMode.Clamp;
        }

        void ShutCubemapTexture()
        {
            if (!cubemapTexture)
            {
                return;
            }

            DestroyImmediate(cubemapTexture);
            cubemapTexture = default;
        }

        // ------------------------------------------------

        RenderTexture cookiesTexture = default;

        // ----------------------------

        void InitCookiesTexture()
        {
            ShutCookiesTexture();

            if (!globalSettings)
            {
                return;
            }

            cookiesTexture = new RenderTexture(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Tex2D,
                colorFormat = RenderTextureFormat.ARGB32,
                width = globalSettings.CookiesTextureSize,
                height = globalSettings.CookiesTextureSize,
                volumeDepth = 1,
                bindMS = false,
                msaaSamples = 1,
                enableRandomWrite = true,
            })
            {
                name = "Clouds Volume Cookies Texture",
            };

            if (!cookiesTexture)
            {
                return;
            }

            cookiesTexture.filterMode = FilterMode.Bilinear;
            cookiesTexture.wrapMode = TextureWrapMode.Repeat;
        }

        void ShutCookiesTexture()
        {
            if (!cookiesTexture)
            {
                return;
            }

            DestroyImmediate(cookiesTexture);
            cookiesTexture = default;
        }

        // ------------------------------------------------

        CloudsVolumeCompute computeOperation = default;

        // ----------------------------

        void InitComputeOperation()
        {
            ShutComputeOperation();

            if (!globalSettings)
            {
                return;
            }

            if (!cubemapTexture)
            {
                return;
            }

            if (!cookiesTexture)
            {
                return;
            }

            computeOperation = new CloudsVolumeCompute(globalSettings, cubemapTexture, cookiesTexture);
        }

        void TickComputeOperation()
        {
            if (computeOperation is null)
            {
                return;
            }

            if (computeOperation.MoveNext())
            {
                return;
            }

            InitComputeOperation();
        }

        void ShutComputeOperation()
        {
            if (computeOperation is null)
            {
                return;
            }

            computeOperation.Dispose();
            computeOperation = default;
        }

        // ------------------------------------------------

        [Header("Reflection Probe")]

        [SerializeField]
        ReflectionProbe reflectionProbe = default;

        // ----------------------------

        const string SHADER_SKYBOX_TEXTURE_PROPERTY = "_SkyboxTexture";

        const string SHADER_SUN_DIR_PROPERTY = "_SunDir";

        const string SHADER_SUN_COL_PROPERTY = "_SunCol";

        // ----------------------------

        void TickReflectionProbe()
        {
            if (!reflectionProbe)
            {
                return;
            }

            reflectionProbe.RenderProbe();
        }

        // ------------------------------------------------

        void SyncRendererProperties()
        {
            SyncRendererProperties_Sky();
            SyncRendererProperties_Sun();
        }

        void SyncRendererProperties_Sky()
        {
            if (!cubemapTexture)
            {
                return;
            }

            var skyboxMaterial = RenderSettings.skybox;
            if (!skyboxMaterial)
            {
                return;
            }

            skyboxMaterial.SetTexture(SHADER_SKYBOX_TEXTURE_PROPERTY, cubemapTexture);
            skyboxMaterial.SetVector(SHADER_SUN_DIR_PROPERTY, math.float4(CloudsVolumeObjects.SunDir, 0f));
            skyboxMaterial.SetVector(SHADER_SUN_COL_PROPERTY, math.float4(CloudsVolumeObjects.SunCol, 1f));
            DynamicGI.UpdateEnvironment();
        }

        void SyncRendererProperties_Sun()
        {
            if (!cookiesTexture)
            {
                return;
            }

            var sunLight = CloudsVolumeObjects.SunLight;
            if (!sunLight)
            {
                return;
            }

            sunLight.cookie = cookiesTexture;
        }

        // ------------------------------------------------
    }
}
