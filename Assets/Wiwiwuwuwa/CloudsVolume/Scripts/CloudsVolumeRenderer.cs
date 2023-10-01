using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using UnityEditor;

namespace Wiwiwuwuwa.CloudsVolume
{
    [ExecuteInEditMode]
    public class CloudsVolumeRenderer : MonoBehaviour
    {
        // ----------------------------------------------------

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

#if UNITY_EDITOR
            EditorApplication.update += TickSystems;
#endif

            if (globalSettings)
            {
                cachedSettings = globalSettings;
                cachedSettings.OnValidateEvent += OnValidate;
            }
        }

#if !UNITY_EDITOR
        void Update()
        {
            TickSystems();
        }
#endif

        void OnDisable()
        {
            if (cachedSettings)
            {
                cachedSettings.OnValidateEvent -= OnValidate;
                cachedSettings = default;
            }

#if UNITY_EDITOR
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.RemoveAll(EditorApplication.update, new EditorApplication.CallbackFunction(TickSystems));
#endif

            ShutSystems();
        }

        // ----------------------------------------------------

        void InitSystems()
        {
            ShutSystems();

            InitCubemapTexture();
            InitComputeOperation();
            InitReflectionProbe();

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
            ShutReflectionProbe();
            ShutComputeOperation();
            ShutCubemapTexture();
        }

        // ----------------------------------------------------

        RenderTexture cubemapTexture = default;

        // --------------------------------

        void InitCubemapTexture()
        {
            ShutCubemapTexture();

            if (!globalSettings)
            {
                return;
            }

            cubemapTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
            {
                dimension = TextureDimension.Cube,
                colorFormat = RenderTextureFormat.ARGB32,
                width = globalSettings.CubemapTextureSize,
                height = globalSettings.CubemapTextureSize,
                volumeDepth = 1,
                bindMS = false,
                msaaSamples = 1,
                enableRandomWrite = true,
            });

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

            RenderTexture.ReleaseTemporary(cubemapTexture);
        }

        // ----------------------------------------------------

        CloudsVolumeCompute computeOperation = default;

        // --------------------------------

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

            computeOperation = new CloudsVolumeCompute(globalSettings, cubemapTexture);
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

        // ----------------------------------------------------

        ReflectionProbe reflectionProbe = default;

        // --------------------------------

        void InitReflectionProbe()
        {
            ShutReflectionProbe();
            reflectionProbe = GetComponent<ReflectionProbe>();
        }

        void TickReflectionProbe()
        {
            if (!reflectionProbe)
            {
                return;
            }

            reflectionProbe.RenderProbe();
        }

        void ShutReflectionProbe()
        {
            reflectionProbe = default;
        }

        // ----------------------------------------------------

        const string SHADER_SKYBOX_TEXTURE_PROPERTY = "_SkyboxTexture";

        const string SHADER_SUN_COL_PROPERTY = "_SunCol";

        const string SHADER_SUN_DIR_PROPERTY = "_SunDir";

        // --------------------------------

        void SyncRendererProperties()
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
            skyboxMaterial.SetVector(SHADER_SUN_COL_PROPERTY, math.float4(CloudsVolumeEnvironment.GetSunColor(), default));
            skyboxMaterial.SetVector(SHADER_SUN_DIR_PROPERTY, math.float4(-CloudsVolumeEnvironment.GetSunForwardVector(), default));
            DynamicGI.UpdateEnvironment();
        }

        // ----------------------------------------------------
    }
}
