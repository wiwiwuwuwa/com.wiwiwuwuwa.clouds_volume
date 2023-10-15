using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    public static class CloudsVolumeEnvironment
    {
        // ----------------------------------------------------

        public static float3 GetSunForward()
        {
            var sunTransform = GetSunTransform();
            if (!sunTransform)
            {
                Debug.LogError($"({nameof(sunTransform)}) is not valid");
                return default;
            }

            return sunTransform.forward;
        }

        public static float3 GetSunColor()
        {
            var sunLight = GetSunLight();
            if (!sunLight)
            {
                Debug.LogError($"({nameof(sunLight)}) is not valid");
                return default;
            }

            var color = sunLight.color.linear * sunLight.intensity;
            color = sunLight.useColorTemperature ? color * Mathf.CorrelatedColorTemperatureToRGB(sunLight.colorTemperature).linear : color;

            return math.float3(color.r, color.g, color.b);
        }

        public static float3 GetEyePosition()
        {
            var eyeTransform = GetEyeTransform();
            if (!eyeTransform)
            {
                Debug.LogError($"({nameof(eyeTransform)}) is not valid");
                return default;
            }

            return eyeTransform.position;
        }

        // ----------------------------

        public static Light GetSunLight()
        {
            return RenderSettings.sun;
        }

        public static Transform GetSunTransform()
        {
            var sunLight = GetSunLight();
            if (!sunLight)
            {
                Debug.LogError($"({nameof(sunLight)}) is not valid");
                return default;
            }

            return sunLight.transform;
        }

        public static Camera GetEyeCamera()
        {
            var editCamera = GetEditCamera();
            if (editCamera)
            {
                return editCamera;
            }

            var gameCamera = GetGameCamera();
            if (gameCamera)
            {
                return gameCamera;
            }

            Debug.LogError($"({nameof(editCamera)}) and ({nameof(gameCamera)}) are not valid");
            return default;
        }

        public static Transform GetEyeTransform()
        {
            var eyeCamera = GetEyeCamera();
            if (!eyeCamera)
            {
                Debug.LogError($"({nameof(eyeCamera)}) is not valid");
                return default;
            }

            return eyeCamera.transform;
        }

        // ----------------------------

        static Camera GetEditCamera()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (!sceneView)
            {
                return default;
            }

            return sceneView.camera;
        }

        static Camera GetGameCamera()
        {
            return Camera.main;
        }

        // ----------------------------------------------------
    }
}
