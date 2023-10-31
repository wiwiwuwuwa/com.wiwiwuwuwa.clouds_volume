using System.Collections;
using UnityEngine;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class CloudsVolumeComputeCookies : ComputeOperation
    {
        // ------------------------------------------------

        const string SHADER_COOKIES_TEXTURE_PROPERTY = "_Wiwiw_CookiesTexture";

        const string SHADER_COOKIE_POSITION_MATRIX_PROPERTY = "_Wiwiw_CookiePositionMatrix";

        const string SHADER_COOKIE_ROTATION_MATRIX_PROPERTY = "_Wiwiw_CookieRotationMatrix";

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

            cookiesShader.SetValues(SHADER_COOKIES_TEXTURE_PROPERTY, cookiesTexture);
            cookiesShader.SetValues(SHADER_COOKIE_POSITION_MATRIX_PROPERTY, CloudsVolumeObjects.CookiePositionMatrix);
            cookiesShader.SetValues(SHADER_COOKIE_ROTATION_MATRIX_PROPERTY, CloudsVolumeObjects.CookieRotationMatrix);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_TEXTURE_PROPERTY, globalSettings.DensityTexture);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_MULTIPLY_PROPERTY, globalSettings.DensityMultiply);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_CONTRAST_PROPERTY, globalSettings.DensityContrast);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_DENSITY_MIDPOINT_PROPERTY, globalSettings.DensityMidpoint);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_GRADIENT_POINTS_PROPERTY, globalSettings.GradientPoints);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_GRADIENT_COLORS_PROPERTY, globalSettings.GradientColors);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MIN_PROPERTY, globalSettings.CloudsHeightMin);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_HEIGHT_MAX_PROPERTY, globalSettings.CloudsHeightMax);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_BENT_NORMAL_SCALE_PROPERTY, globalSettings.BentNormalScale);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_STEP_DENSITY_PROPERTY, globalSettings.CloudsSampleStepDensity);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_FULL_DISTANCE_PROPERTY, globalSettings.CloudsSampleFullDistance);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_FLT_PROPERTY, globalSettings.CloudsSampleNumberFlt);
            cookiesShader.SetValues(SHADER_LIB_CLOUDS_CLOUDS_SAMPLE_NUMBER_RCP_PROPERTY, globalSettings.CloudsSampleNumberRcp);

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
