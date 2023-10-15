using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Wiwiwuwuwa.CloudsVolume;

public class Test : MonoBehaviour
{
    [SerializeField]
    CloudsVolumeGlobalSettings globalSettings = default;

    [SerializeField]
    RenderTexture densityTexture = default;

    [SerializeField]
    RenderTexture shadowsTexture = default;

    [SerializeField]
    RenderTexture cubemapTexture = default;

    void OnEnable()
    {
        densityTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
        {
            dimension = TextureDimension.Tex3D,
            colorFormat = RenderTextureFormat.R8,
            width = globalSettings.CloudsTextureSize,
            height = globalSettings.CloudsTextureSize,
            volumeDepth = globalSettings.CloudsTextureSize,
            bindMS = false,
            msaaSamples = 1,
            enableRandomWrite = true,
        });

        densityTexture.filterMode = FilterMode.Bilinear;
        densityTexture.wrapMode = TextureWrapMode.Repeat;

        shadowsTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
        {
            dimension = TextureDimension.Tex3D,
            colorFormat = RenderTextureFormat.R8,
            width = globalSettings.ShadowTextureSize,
            height = globalSettings.ShadowTextureSize,
            volumeDepth = globalSettings.ShadowTextureSize,
            bindMS = false,
            msaaSamples = 1,
            enableRandomWrite = true,
        });

        shadowsTexture.filterMode = FilterMode.Bilinear;
        shadowsTexture.wrapMode = TextureWrapMode.Clamp;

        cubemapTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
        {
            dimension = TextureDimension.Cube,
            colorFormat = RenderTextureFormat.ARGB32,
            width = globalSettings.SkyboxTextureSize,
            height = globalSettings.SkyboxTextureSize,
            volumeDepth = 1,
            bindMS = false,
            msaaSamples = 1,
            enableRandomWrite = true,
        });

        cubemapTexture.filterMode = FilterMode.Bilinear;
        cubemapTexture.wrapMode = TextureWrapMode.Clamp;
    }

    void Update()
    {
        var densityCompute = new CloudsVolumeComputeDensity(globalSettings, densityTexture);
        while (densityCompute.MoveNext()) { }

        var shadowsCompute = new CloudsVolumeComputeShadows(globalSettings, densityTexture, shadowsTexture);
        while (shadowsCompute.MoveNext()) { }

        var cubemapCompute = new CloudsVolumeComputeCubemap(globalSettings, densityTexture, shadowsTexture, cubemapTexture);
        while (cubemapCompute.MoveNext()) { }
    }

    void OnDisable()
    {
        RenderTexture.ReleaseTemporary(shadowsTexture);
        RenderTexture.ReleaseTemporary(densityTexture);
    }
}
