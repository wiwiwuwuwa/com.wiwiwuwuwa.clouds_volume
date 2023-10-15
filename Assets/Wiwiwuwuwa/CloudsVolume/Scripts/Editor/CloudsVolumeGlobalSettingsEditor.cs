// using UnityEngine;
// using UnityEditor;
// using Unity.Mathematics;

// namespace Wiwiwuwuwa.CloudsVolume
// {
//     [CustomEditor(typeof(CloudsVolumeGlobalSettings))]
//     public class CloudsVolumeGlobalSettingsEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             UpdateSerializedObject();

//             EditorGUILayout.LabelField("Clouds Settings", EditorStyles.boldLabel);
//             DrawCloudsShaderProp();
//             DrawCloudsTextureSizeProp();
//             DrawCloudsAreaAngleProp();
//             DrawCloudsGradientParamsProp();
//             DrawCloudsNoiseScaleProp();
//             DrawCloudsContrastProp();
//             DrawCloudsMidpointProp();

//             EditorGUILayout.Space();

//             EditorGUILayout.LabelField("Shadow Settings", EditorStyles.boldLabel);
//             DrawShadowShaderProp();
//             DraWShadowTextureSizeProp();
//             DrawShadowAreaAngleProp();
//             DrawShadowAreaPowerProp();
//             DrawShadowSamplesProp();
//             DrawShadowDensityProp();

//             EditorGUILayout.Space();

//             EditorGUILayout.LabelField("Cubemap Settings", EditorStyles.boldLabel);
//             // DrawCubemapComputeShaderProp();
//             // DrawCubemapTextureSizeProp();
//             // DrawCubemapSamplesProp();
//             // DrawCubemapDensityProp();

//             EditorGUILayout.Space();

//             EditorGUILayout.LabelField("Cookies Settings", EditorStyles.boldLabel);
//             // DrawCookiesComputeShaderProp();
//             // DrawCookiesTextureSizeProp();

//             ApplySerializedObject();
//         }

//         void UpdateSerializedObject()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is not valid");
//                 return;
//             }

//             serializedObject.Update();
//         }

//         void DrawCloudsShaderProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsShader");
//             if (serializedProperty is null)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             EditorGUILayout.PropertyField(serializedProperty);
//         }

//         void DrawCloudsTextureSizeProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsTextureSize");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.intValue;
//             propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//             propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
//             serializedProperty.intValue = propertyValue;
//         }

//         void DrawCloudsAreaAngleProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsAreaAngle");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.floatValue;
//             propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 90f);
//             serializedProperty.floatValue = propertyValue;
//         }

//         void DrawCloudsGradientParamsProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsGradientParams");
//             if (serializedProperty is null || serializedProperty.propertyType != SerializedPropertyType.Vector4)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.vector4Value;
//             propertyValue = EditorGUILayout.Vector4Field(serializedProperty.displayName, propertyValue);
//             propertyValue = math.max(default, propertyValue);
//             serializedProperty.vector4Value = propertyValue;
//         }

//         void DrawCloudsNoiseScaleProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsNoiseScale");
//             if (serializedProperty is null || serializedProperty.propertyType != SerializedPropertyType.Vector4)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.vector4Value;
//             propertyValue = EditorGUILayout.Vector4Field(serializedProperty.displayName, propertyValue);
//             propertyValue = math.max(default, propertyValue);
//             serializedProperty.vector4Value = propertyValue;
//         }

//         void DrawCloudsContrastProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsContrast");
//             if (serializedProperty is null || serializedProperty.propertyType != SerializedPropertyType.Vector4)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.vector4Value;
//             propertyValue = EditorGUILayout.Vector4Field(serializedProperty.displayName, propertyValue);
//             propertyValue = math.max(default, propertyValue);
//             serializedProperty.vector4Value = propertyValue;
//         }

//         void DrawCloudsMidpointProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("cloudsMidpoint");
//             if (serializedProperty is null || serializedProperty.propertyType != SerializedPropertyType.Vector4)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.vector4Value;
//             propertyValue = EditorGUILayout.Vector4Field(serializedProperty.displayName, propertyValue);
//             propertyValue = math.max(default, propertyValue);
//             serializedProperty.vector4Value = propertyValue;
//         }

//         void DrawShadowShaderProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowShader");
//             if (serializedProperty is null)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             EditorGUILayout.PropertyField(serializedProperty);
//         }

//         void DraWShadowTextureSizeProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowTextureSize");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.intValue;
//             propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//             propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
//             serializedProperty.intValue = propertyValue;
//         }

//         void DrawShadowAreaAngleProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowAreaAngle");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.floatValue;
//             propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 90f);
//             serializedProperty.floatValue = propertyValue;
//         }

//         void DrawShadowAreaPowerProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowAreaPower");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.floatValue;
//             propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 10f);
//             serializedProperty.floatValue = propertyValue;
//         }

//         void DrawShadowSamplesProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowSamples");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.intValue;
//             propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//             propertyValue = math.max(2, propertyValue);
//             serializedProperty.intValue = propertyValue;
//         }

//         void DrawShadowDensityProp()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is null");
//                 return;
//             }

//             var serializedProperty = serializedObject.FindProperty("shadowDensity");
//             if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
//             {
//                 Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//                 return;
//             }

//             var propertyValue = serializedProperty.floatValue;
//             propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
//             serializedProperty.floatValue = propertyValue;
//         }

//         // void DrawCubemapComputeShaderProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is not valid");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cubemapComputeShader");
//         //     if (serializedProperty is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     EditorGUILayout.PropertyField(serializedProperty);
//         // }

//         // void DrawCubemapTextureSizeProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is not valid");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cubemapTextureSize");
//         //     if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     var propertyValue = serializedProperty.intValue;
//         //     propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//         //     propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
//         //     serializedProperty.intValue = propertyValue;
//         // }

//         // void DrawCubemapSamplesProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is null");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cubemapSamples");
//         //     if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     var propertyValue = serializedProperty.intValue;
//         //     propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//         //     propertyValue = math.max(2, propertyValue);
//         //     serializedProperty.intValue = propertyValue;
//         // }

//         // void DrawCubemapDensityProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is null");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cubemapDensity");
//         //     if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Float)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     var propertyValue = serializedProperty.floatValue;
//         //     propertyValue = EditorGUILayout.Slider(serializedProperty.displayName, propertyValue, 0f, 1f);
//         //     serializedProperty.floatValue = propertyValue;
//         // }

//         // void DrawCookiesComputeShaderProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is null");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cookiesComputeShader");
//         //     if (serializedProperty is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     EditorGUILayout.PropertyField(serializedProperty);
//         // }

//         // void DrawCookiesTextureSizeProp()
//         // {
//         //     var serializedObject = this.serializedObject;
//         //     if (serializedObject is null)
//         //     {
//         //         Debug.LogError($"({nameof(serializedObject)}) is null");
//         //         return;
//         //     }

//         //     var serializedProperty = serializedObject.FindProperty("cookiesTextureSize");
//         //     if (serializedProperty is null || serializedProperty.numericType != SerializedPropertyNumericType.Int32)
//         //     {
//         //         Debug.LogError($"({nameof(serializedProperty)}) is not valid");
//         //         return;
//         //     }

//         //     var propertyValue = serializedProperty.intValue;
//         //     propertyValue = EditorGUILayout.IntField(serializedProperty.displayName, propertyValue);
//         //     propertyValue = math.ceilpow2(math.clamp(propertyValue, 2, 1024));
//         //     serializedProperty.intValue = propertyValue;
//         // }

//         void ApplySerializedObject()
//         {
//             var serializedObject = this.serializedObject;
//             if (serializedObject is null)
//             {
//                 Debug.LogError($"({nameof(serializedObject)}) is not valid");
//                 return;
//             }

//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }
