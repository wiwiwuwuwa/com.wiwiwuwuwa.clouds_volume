using UnityEngine;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    public static class CloudsVolumeEnvironment
    {
        // ----------------------------------------------------

        public static float4x4 GetSunObjectToWorldMatrix()
        {
            var sunTransform = GetSunTranformComponent();
            if (!sunTransform)
            {
                Debug.LogError($"({nameof(sunTransform)}) is not valid");
                return float4x4.identity;
            }

            return float4x4.LookAt(default, GetSunForwardVector(), GetSunUpwardsVector());
        }

        public static float3 GetSunForwardVector()
        {
            var sunTransform = GetSunTranformComponent();
            if (!sunTransform)
            {
                Debug.LogError($"({nameof(sunTransform)}) is not valid");
                return default;
            }

            return sunTransform.forward;
        }

        public static float3 GetSunUpwardsVector()
        {
            var sunTransform = GetSunTranformComponent();
            if (!sunTransform)
            {
                Debug.LogError($"({nameof(sunTransform)}) is not valid");
                return default;
            }

            return sunTransform.up;
        }

        public static float3 GetSunColor()
        {
            var sunLight = GetSunLightComponent();
            if (!sunLight)
            {
                Debug.LogError($"({nameof(sunLight)}) is not valid");
                return default;
            }

            var color = sunLight.color.linear * sunLight.intensity;
            color = sunLight.useColorTemperature ? color * Mathf.CorrelatedColorTemperatureToRGB(sunLight.colorTemperature).linear : color;

            return math.float3(color.r, color.g, color.b);
        }

        // ----------------------------

        static Light GetSunLightComponent()
        {
            return RenderSettings.sun;
        }

        static Transform GetSunTranformComponent()
        {
            var sunLight = GetSunLightComponent();
            if (!sunLight)
            {
                Debug.LogError($"({nameof(sunLight)}) is not valid");
                return default;
            }

            return sunLight.transform;
        }

        // ----------------------------------------------------
    }
}
