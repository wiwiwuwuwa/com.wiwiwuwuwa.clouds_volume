using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    public static class CloudsVolumeObjects
    {
        // ------------------------------------------------

        public static float3 CameraPosition
        {
            get
            {
                var eyeTransform = EyeTransform;
                if (!eyeTransform)
                {
                    Debug.LogError($"({nameof(eyeTransform)}) is not valid");
                    return default;
                }

                return eyeTransform.position;
            }
        }

        public static float3 SunDir
        {
            get
            {
                var sunTransform = SunTransform;
                if (!sunTransform)
                {
                    Debug.LogError($"({nameof(sunTransform)}) is not valid");
                    return default;
                }

                return sunTransform.forward;
            }
        }

        public static float3 SunCol
        {
            get
            {
                var sunLight = SunLight;
                if (!sunLight)
                {
                    Debug.LogError($"({nameof(sunLight)}) is not valid");
                    return default;
                }

                var sunColor = sunLight.color.linear * sunLight.intensity;
                sunColor = sunLight.useColorTemperature ? sunColor * Mathf.CorrelatedColorTemperatureToRGB(sunLight.colorTemperature).linear : sunColor;

                return math.float3(sunColor.r, sunColor.g, sunColor.b);
            }
        }

        public static float4x4 CookiePositionMatrix
        {
            get
            {
                var sunLight = SunLight;
                if (!sunLight)
                {
                    Debug.LogError($"({nameof(sunLight)}) is not valid");
                    return default;
                }

                var sunTransform = SunTransform;
                if (!sunTransform)
                {
                    Debug.LogError($"({nameof(sunTransform)}) is not valid");
                    return default;
                }

                var matrix = float4x4.identity;
                matrix = math.mul(float4x4.Translate(-0.5f), matrix);
                matrix = math.mul(float4x4.Scale(sunLight.cookieSize), matrix);
                matrix = math.mul(float4x4.LookAt(default, sunTransform.forward, sunTransform.up), matrix);
                matrix = math.mul(float4x4.Translate(sunTransform.position), matrix);
                return matrix;
            }
        }

        public static float4x4 CookieRotationMatrix
        {
            get
            {
                var sunLight = SunLight;
                if (!sunLight)
                {
                    Debug.LogError($"({nameof(sunLight)}) is not valid");
                    return default;
                }

                var sunTransform = SunTransform;
                if (!sunTransform)
                {
                    Debug.LogError($"({nameof(sunTransform)}) is not valid");
                    return default;
                }

                var matrix = float4x4.identity;
                matrix = math.mul(float4x4.LookAt(default, sunTransform.forward, sunTransform.up), matrix);
                return matrix;
            }
        }

        // ------------------------------------------------

        public static Transform EyeTransform
        {
            get
            {
                var eyeCamera = EyeCamera;
                if (!eyeCamera)
                {
                    Debug.LogError($"({nameof(eyeCamera)}) is not valid");
                    return default;
                }

                return eyeCamera.transform;
            }
        }

        public static Camera EyeCamera
        {
            get
            {
                if (Application.isPlaying)
                {
                    return EyeCameraForPlayMode;
                }
                else
                {
                    return EyeCameraForEditMode;
                }
            }
        }

        public static Transform SunTransform
        {
            get
            {
                var sunLight = SunLight;
                if (!sunLight)
                {
                    Debug.LogError($"({nameof(sunLight)}) is not valid");
                    return default;
                }

                return sunLight.transform;
            }
        }

        public static Light SunLight
        {
            get
            {
                return RenderSettings.sun;
            }
        }

        // ----------------------------

        static Camera EyeCameraForEditMode
        {
            get
            {
                var sceneView = SceneView.lastActiveSceneView;
                if (!sceneView)
                {
                    return default;
                }

                return sceneView.camera;
            }
        }

        static Camera EyeCameraForPlayMode
        {
            get
            {
                return Camera.main;
            }
        }

        // ------------------------------------------------
    }
}
