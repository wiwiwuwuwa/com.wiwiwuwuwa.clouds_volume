using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    [CustomEditor(typeof(CloudsVolumeGlobalSettings))]
    public class CloudsVolumeGlobalSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UpdateSerializedObject();

            EditorGUILayout.LabelField("Density Settings", EditorStyles.boldLabel);
            DrawDensityComputeShaderProp();
            DrawDensityTextureSizeProp();
            DrawDensityNoiseScaleProp();
            DrawDensityContrastProp();
            DrawDensityMidpointProp();
            DrawDensityFadePosProp();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Shadows Settings", EditorStyles.boldLabel);
            DrawShadowsComputeShaderProp();
            DrawShadowsTextureSizeProp();
            DrawShadowsAreaScaleProp();
            DrawShadowsSamplesProp();
            DrawShadowsDensityProp();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cubemap Settings", EditorStyles.boldLabel);
            DrawCubemapComputeShaderProp();
            DrawCubemapTextureSizeProp();
            DrawCubemapSamplesProp();
            DrawCubemapDensityProp();

            ApplySerializedObject();
        }

        void UpdateSerializedObject()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            serializedObject.Update();
        }

        void DrawDensityComputeShaderProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("densityComputeShader");
            if (serializedProperty is null)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            EditorGUILayout.PropertyField(serializedProperty);
        }

        void DrawDensityTextureSizeProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("densityTextureSize");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.intValue;
            propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
            propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
            serializedProperty.intValue = propertyValue;
        }

        void DrawDensityNoiseScaleProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("densityNoiseScale");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.FloatField(serializedProperty.displayName, propertyValue);
            propertyValue = math.max(0f, propertyValue);
            serializedProperty.floatValue = propertyValue;
        }

        void DrawDensityContrastProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("densityContrast");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
            serializedProperty.floatValue = propertyValue;
        }

        void DrawDensityMidpointProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("densityMidpoint");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
            serializedProperty.floatValue = propertyValue;
        }

        void DrawDensityFadePosProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var fadeInStart = serializedObject.FindProperty("densityFadeInStartPos");
            var fadeInFinal = serializedObject.FindProperty("densityFadeInFinalPos");
            var fadeOutStart = serializedObject.FindProperty("densityFadeOutStartPos");
            var fadeOutFinal = serializedObject.FindProperty("densityFadeOutFinalPos");

            if (fadeInStart is null || fadeInFinal is null || fadeOutStart is null || fadeOutFinal is null)
            {
                Debug.LogError($"(densityFadePos) is not valid");
                return;
            }

            var propertyValue = new float4(fadeInStart.floatValue, fadeInFinal.floatValue, fadeOutStart.floatValue, fadeOutFinal.floatValue);
            propertyValue = EditorGUILayout.Vector4Field("Density Fade Pos", propertyValue);
            propertyValue = math.clamp(propertyValue, 0f, 1f);

            fadeInStart.floatValue = propertyValue.x;
            fadeInFinal.floatValue = propertyValue.y;
            fadeOutStart.floatValue = propertyValue.z;
            fadeOutFinal.floatValue = propertyValue.w;
        }

        void DrawShadowsComputeShaderProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("shadowsComputeShader");
            if (serializedProperty is null)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            EditorGUILayout.PropertyField(serializedProperty);
        }

        void DrawShadowsTextureSizeProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("shadowsTextureSize");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.intValue;
            propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
            propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
            serializedProperty.intValue = propertyValue;
        }

        void DrawShadowsAreaScaleProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is null");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("shadowsAreaScale");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 1f, 8f);
            serializedProperty.floatValue = propertyValue;
        }

        void DrawShadowsSamplesProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("shadowsSamples");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.intValue;
            propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
            propertyValue = math.max(2, propertyValue);
            serializedProperty.intValue = propertyValue;
        }

        void DrawShadowsDensityProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("shadowsDensity");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
            serializedProperty.floatValue = propertyValue;
        }

        void DrawCubemapComputeShaderProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("cubemapComputeShader");
            if (serializedProperty is null)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            EditorGUILayout.PropertyField(serializedProperty);
        }

        void DrawCubemapTextureSizeProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("cubemapTextureSize");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.intValue;
            propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
            propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
            serializedProperty.intValue = propertyValue;
        }

        void DrawCubemapSamplesProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is null");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("cubemapSamples");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.intValue;
            propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
            propertyValue = math.max(2, propertyValue);
            serializedProperty.intValue = propertyValue;
        }

        void DrawCubemapDensityProp()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is null");
                return;
            }

            var serializedProperty = serializedObject.FindProperty("cubemapDensity");
            if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
            {
                Debug.LogError($"({nameof(serializedProperty)}) is not valid");
                return;
            }

            var propertyValue = serializedProperty.floatValue;
            propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
            serializedProperty.floatValue = propertyValue;
        }

        void ApplySerializedObject()
        {
            var serializedObject = this.serializedObject;
            if (serializedObject is null)
            {
                Debug.LogError($"({nameof(serializedObject)}) is not valid");
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
