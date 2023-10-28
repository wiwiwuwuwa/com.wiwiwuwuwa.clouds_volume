using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCubemap : ComputeOperation
    {
        // ------------------------------------------------

        const string SHADER_CUBEMAP_FACE_TEXTURE_PROPERTY = "_Wiwiw_CubemapFaceTexture";

        const string SHADER_CUBEMAP_FACE_INDEX_PROPERTY = "_Wiwiw_CubemapFaceIndex";

        const string SHADER_LIB_CLOUDS_DENSITY_TEXTURE_PROPERTY = "_Wiwiw_LibClouds_DensityTexture";

        const string SHADER_LIB_CLOUDS_DENSITY_MULTIPLY_PROPERTY = "_Wiwiw_LibClouds_DensityMultiply";

        const string SHADER_LIB_CLOUDS_DENSITY_CONTRAST_PROPERTY = "_Wiwiw_LibClouds_DensityContrast";

        const string SHADER_LIB_CLOUDS_DENSITY_MIDPOINT_PROPERTY = "_Wiwiw_LibClouds_DensityMidpoint";

        const string SHADER_LIB_CLOUDS_GRADIENT_POINTS_PROPERTY = "_Wiwiw_LibClouds_GradientPoints";

        const string SHADER_LIB_CLOUDS_GRADIENT_COLORS_PROPERTY = "_Wiwiw_LibClouds_GradientColors";

        const string SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MIN_PROPERTY = "_Wiwiw_LibClouds_CloudsHeightMin";

        const string SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MAX_PROPERTY = "_Wiwiw_LibClouds_CloudsHeightMax";

        const string SHADER_LIB_CLOUDS_BENT_NORMAL_SCALE_PROPERTY = "_Wiwiw_LibClouds_BentNormalScale";

        const string SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_STEP_DENSITY_PROPERTY = "_Wiwiw_LibClouds_CloudsSampleStepDensity";

        const string SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_FULL_DISTANCE_PROPERTY = "_Wiwiw_LibClouds_CloudsSampleFullDistance";

        const string SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_FLT_PROPERTY = "_Wiwiw_LibClouds_CloudsSampleNumberFlt";

        const string SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_RCP_PROPERTY = "_Wiwiw_LibClouds_CloudsSampleNumberRcp";

        const string SHADER_CAMERA_POSITION_PROPERTY = "_Wiwiw_CameraPosition";

        // ----------------------------

        readonly CloudsVolumeGlobalSettings globalSettings = default;

        readonly RenderTexture cubemapTexture = default;

        // ----------------------------

        public CloudsVolumeComputeCubemap(CloudsVolumeGlobalSettings globalSettings, RenderTexture cubemapTexture)
        {
            this.globalSettings = globalSettings;
            this.cubemapTexture = cubemapTexture;
        }

        protected override IEnumerator Execute()
        {
            if (!globalSettings)
            {
                Debug.LogError($"({nameof(globalSettings)}) is not valid");
                yield break;
            }

            if (!globalSettings.CubemapShader)
            {
                Debug.LogError($"({nameof(globalSettings.CubemapShader)}) is not valid");
                yield break;
            }

            if (!globalSettings.DensityTexture)
            {
                Debug.LogError($"({nameof(globalSettings.DensityTexture)}) is not valid");
                yield break;
            }

            if (!cubemapTexture)
            {
                Debug.LogError($"({nameof(cubemapTexture)}) is not valid");
                yield break;
            }

            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_TEXTURE_PROPERTY, globalSettings.DensityTexture);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_MULTIPLY_PROPERTY, globalSettings.DensityMultiply);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_CONTRAST_PROPERTY, globalSettings.DensityContrast);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_MIDPOINT_PROPERTY, globalSettings.DensityMidpoint);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_GRADIENT_POINTS_PROPERTY, globalSettings.GradientPoints);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_GRADIENT_COLORS_PROPERTY, globalSettings.GradientColors);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MIN_PROPERTY, globalSettings.CloudsHeightMin);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MAX_PROPERTY, globalSettings.CloudsHeightMax);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_BENT_NORMAL_SCALE_PROPERTY, globalSettings.BentNormalScale);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_STEP_DENSITY_PROPERTY, globalSettings.CloudsSampleStepDensity);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_FULL_DISTANCE_PROPERTY, globalSettings.CloudsSampleFullDistance);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_FLT_PROPERTY, globalSettings.CloudsSampleNumberFlt);
            globalSettings.CubemapShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_RCP_PROPERTY, globalSettings.CloudsSampleNumberRcp);
            globalSettings.CubemapShader.SetValues(SHADER_CAMERA_POSITION_PROPERTY, CloudsVolumeObjects.CameraPosition);

            for (var cubemapFaceIndex = 0; cubemapFaceIndex < 6; cubemapFaceIndex++)
            {
                var cubemapFaceTexture = RenderTexture.GetTemporary(new RenderTextureDescriptor
                {
                    dimension = TextureDimension.Tex2D,
                    colorFormat = cubemapTexture.format,
                    width = cubemapTexture.width,
                    height = cubemapTexture.height,
                    volumeDepth = 1,
                    bindMS = false,
                    msaaSamples = 1,
                    enableRandomWrite = true,
                });

                if (!cubemapFaceTexture)
                {
                    Debug.LogError($"({nameof(cubemapFaceTexture)}) is not valid");
                    yield break;
                }

                Defer(() => RenderTexture.ReleaseTemporary(cubemapFaceTexture));

                globalSettings.CubemapShader.SetValues(SHADER_CUBEMAP_FACE_TEXTURE_PROPERTY, cubemapFaceTexture);
                globalSettings.CubemapShader.SetValues(SHADER_CUBEMAP_FACE_INDEX_PROPERTY, cubemapFaceIndex);

                var dispatchOperation = new DispatchComputeShader(globalSettings.CubemapShader, cubemapTexture.GetSize());
                if (dispatchOperation is null)
                {
                    Debug.LogError($"({nameof(dispatchOperation)}) is not valid");
                    yield break;
                }

                while (dispatchOperation.MoveNext()) yield return default;

                Graphics.CopyTexture
                (
                    src: cubemapFaceTexture,
                    srcElement: default,
                    srcMip: default,
                    dst: cubemapTexture,
                    dstElement: cubemapFaceIndex,
                    dstMip: default
                );

                RenderTexture.ReleaseTemporary(cubemapFaceTexture);
                yield return default;
            }
        }

        // ------------------------------------------------
    }
}
