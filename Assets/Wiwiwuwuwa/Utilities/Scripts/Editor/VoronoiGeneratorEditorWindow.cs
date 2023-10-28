using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Wiwiwuwuwa.Utilities
{
    public class VoronoiGeneratorEditorWindow : EditorWindow
    {
        // ------------------------------------------------

        int textureSize = default;

        int cellsNumber = default;

        // ----------------------------

        [MenuItem(itemName: "Window/Wiwiwuwuwa/Utilities/Voronoi Generator", priority = int.MinValue)]
        static void ShowWindow()
        {
            GetWindow<VoronoiGeneratorEditorWindow>("Voronoi Generator");
        }

        void OnEnable()
        {
            textureSize = 128;
            cellsNumber = 12;
        }

        void OnGUI()
        {
            textureSize = EditorGUILayout.DelayedIntField("Texture Size", textureSize);
            textureSize = math.ceilpow2(math.max(4, textureSize));

            cellsNumber = EditorGUILayout.DelayedIntField("Cells Number", cellsNumber);
            cellsNumber = math.max(1, cellsNumber);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Generate"))
                {
                    var filename = EditorUtility.SaveFilePanelInProject("Save Voronoi Texture", "Voronoi", "asset", "Save Voronoi Texture");
                    var filedata = !string.IsNullOrEmpty(filename) ? GenerateVoronoiTexture(textureSize, cellsNumber) : default;

                    if (filedata)
                    {
                        AssetDatabase.CreateAsset(filedata, filename);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        // ----------------------------

        static Texture3D GenerateVoronoiTexture(int pixelsCount, int pointsCount)
        {
            if (pixelsCount <= 0)
            {
                Debug.LogError($"({nameof(pixelsCount)}) is not valid");
                return default;
            }

            if (pointsCount <= 0)
            {
                Debug.LogError($"({nameof(pointsCount)}) is not valid");
                return default;
            }

            var voronoiTexture = new Texture3D
            (
                width: pixelsCount,
                height: pixelsCount,
                depth: pixelsCount,
                textureFormat: TextureFormat.R8,
                mipChain: false,
                createUninitialized: true
            )
            {
                name = "Voronoi",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat,
            };

            if (!voronoiTexture)
            {
                Debug.LogError($"({nameof(voronoiTexture)}) is not valid");
                return default;
            }

            var pixelsArray = GenerateVoronoiPixels(pixelsCount, pointsCount);
            if (pixelsArray is null)
            {
                Debug.LogError($"({nameof(pixelsArray)}) is not valid");
                DestroyImmediate(voronoiTexture);
                return default;
            }

            for (var x = 0; x < pixelsCount; x++)
            {
                for (var y = 0; y < pixelsCount; y++)
                {
                    for (var z = 0; z < pixelsCount; z++)
                    {
                        voronoiTexture.SetPixel(x, y, z, Color.white * pixelsArray[x, y, z]);

                        var currentProgress = (float)(x * pixelsCount * pixelsCount + y * pixelsCount + z) / (pixelsCount * pixelsCount * pixelsCount);
                        EditorUtility.DisplayProgressBar("Setting Voronoi Pixels", default, currentProgress);
                    }
                }
            }
            voronoiTexture.Apply();
            EditorUtility.ClearProgressBar();

            return voronoiTexture;
        }

        static float[,,] GenerateVoronoiPixels(int pixelsCount, int pointsCount)
        {
            if (pixelsCount <= 0)
            {
                Debug.LogError($"({nameof(pixelsCount)}) is not valid");
                return default;
            }

            if (pointsCount <= 0)
            {
                Debug.LogError($"({nameof(pointsCount)}) is not valid");
                return default;
            }

            var pixelsArray = new float[pixelsCount, pixelsCount, pixelsCount];
            if (pixelsArray is null)
            {
                Debug.LogError($"({nameof(pixelsArray)}) is not valid");
                return default;
            }

            var pointsArray = GenerateVoronoiPoints(pointsCount);
            if (pointsArray is null)
            {
                Debug.LogError($"({nameof(pointsArray)}) is not valid");
                return default;
            }

            var pixelsRecip = math.rcp(pixelsCount);
            var maximalDistance = float.Epsilon;

            for (var x = 0; x < pixelsCount; x++)
            {
                for (var y = 0; y < pixelsCount; y++)
                {
                    for (var z = 0; z < pixelsCount; z++)
                    {
                        var point = (math.float3(x, y, z) + 0.5f) * pixelsRecip;
                        var currentDistance = ComputeVoronoiDistance(pointsArray, point);

                        pixelsArray[x, y, z] = currentDistance;
                        maximalDistance = math.max(maximalDistance, currentDistance);

                        var currentProgress = (float)(x * pixelsCount * pixelsCount + y * pixelsCount + z) / (pixelsCount * pixelsCount * pixelsCount);
                        EditorUtility.DisplayProgressBar("Generating Voronoi Pixels", default, currentProgress);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            var maximalDistanceRecip = maximalDistance > 0f ? math.rcp(maximalDistance) : 0f;

            for (var x = 0; x < pixelsCount; x++)
            {
                for (var y = 0; y < pixelsCount; y++)
                {
                    for (var z = 0; z < pixelsCount; z++)
                    {
                        pixelsArray[x, y, z] = math.saturate(pixelsArray[x, y, z] * maximalDistanceRecip);

                        var currentProgress = (float)(x * pixelsCount * pixelsCount + y * pixelsCount + z) / (pixelsCount * pixelsCount * pixelsCount);
                        EditorUtility.DisplayProgressBar("Normalizing Voronoi Pixels", default, currentProgress);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            return pixelsArray;
        }

        static float3[,,] GenerateVoronoiPoints(int pointsCount)
        {
            if (pointsCount <= 0)
            {
                Debug.LogError($"({nameof(pointsCount)}) is not valid");
                return default;
            }

            var pointsArray = new float3[pointsCount, pointsCount, pointsCount];
            if (pointsArray is null)
            {
                Debug.LogError($"({nameof(pointsArray)}) is not valid");
                return default;
            }

            var random = new Unity.Mathematics.Random((uint)Time.frameCount);
            var pointsRecip = math.rcp(pointsCount);

            for (var x = 0; x < pointsCount; x++)
            {
                for (var y = 0; y < pointsCount; y++)
                {
                    for (var z = 0; z < pointsCount; z++)
                    {
                        var cursor = math.float3(x, y, z);
                        var minimum = (cursor + 0f) * pointsRecip;
                        var maximum = (cursor + 1f) * pointsRecip;
                        pointsArray[x, y, z] = random.NextFloat3(minimum, maximum);

                        var currentProgress = (float)(x * pointsCount * pointsCount + y * pointsCount + z) / (pointsCount * pointsCount * pointsCount);
                        EditorUtility.DisplayProgressBar("Generating Voronoi Points", default, currentProgress);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            return pointsArray;
        }

        static float ComputeVoronoiDistance(float3[,,] points, float3 point)
        {
            if (points is null)
            {
                Debug.LogError($"({nameof(points)}) is not valid");
                return default;
            }

            if (math.any(point < 0f) || math.any(point > 1f))
            {
                Debug.LogError($"({nameof(point)}) is not valid");
                return default;
            }

            var pointsCount = math.int3(points.GetLength(0), points.GetLength(1), points.GetLength(2));
            if (math.any(pointsCount <= 0))
            {
                Debug.LogError($"({nameof(pointsCount)}) is not valid");
                return default;
            }

            var pointsCountRecip = math.rcp(pointsCount);
            var center = math.int3(math.floor(point * pointsCount));
            var minimum = center - 1;
            var maximum = center + 1;

            var minimalDistance = float.MaxValue;

            for (var x = minimum.x; x <= maximum.x; x++)
            {
                for (var y = minimum.y; y <= maximum.y; y++)
                {
                    for (var z = minimum.z; z <= maximum.z; z++)
                    {
                        var defaultCursor = math.int3(x, y, z);
                        var wrappedCursor = MathUtils.Wrap(defaultCursor, 0, pointsCount - 1);

                        var defaultPoint = points[wrappedCursor.x, wrappedCursor.y, wrappedCursor.z];
                        var wrappedPoint = defaultPoint + (defaultCursor - wrappedCursor) * pointsCountRecip;

                        var currentDistance = math.distance(point, wrappedPoint);
                        minimalDistance = math.min(minimalDistance, currentDistance);
                    }
                }
            }

            return minimalDistance;
        }

        // ------------------------------------------------
    }
}
