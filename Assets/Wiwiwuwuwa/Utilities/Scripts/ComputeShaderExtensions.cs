using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Wiwiwuwuwa.Utilities
{
    public static class ComputeShaderExtensions
    {
        // ------------------------------------------------

        public static void SetValues(this ComputeShader shader, string name, float value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetFloat(name, value);
        }

        public static void SetValues(this ComputeShader shader, string name, float2 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y);
        }

        public static void SetValues(this ComputeShader shader, string name, float3 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y, value.z);
        }

        public static void SetValues(this ComputeShader shader, string name, float4 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y, value.z, value.w);
        }

        public static void SetValues(this ComputeShader shader, string name, int value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetInt(name, value);
        }

        public static void SetValues(this ComputeShader shader, string name, int2 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetInts(name, value.x, value.y);
        }

        public static void SetValues(this ComputeShader shader, string name, int3 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetInts(name, value.x, value.y, value.z);
        }

        public static void SetValues(this ComputeShader shader, string name, int4 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetInts(name, value.x, value.y, value.z, value.w);
        }

        public static void SetValues(this ComputeShader shader, string name, params float[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetFloats(name, values.SelectMany(v => new[] { v, 0f, 0f, 0f }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params float2[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetFloats(name, values.SelectMany(v => new[] { v.x, v.y, 0f, 0f }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params float3[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetFloats(name, values.SelectMany(v => new[] { v.x, v.y, v.z, 0f }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params float4[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetFloats(name, values.SelectMany(v => new[] { v.x, v.y, v.z, v.w }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params int[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetInts(name, values.SelectMany(v => new[] { v, 0, 0, 0 }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params int2[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetInts(name, values.SelectMany(v => new[] { v.x, v.y, 0, 0 }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params int3[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetInts(name, values.SelectMany(v => new[] { v.x, v.y, v.z, 0 }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, params int4[] values)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (values is null || values.Length == default)
            {
                Debug.LogError($"({nameof(values)}) is not valid");
                return;
            }

            shader.SetInts(name, values.SelectMany(v => new[] { v.x, v.y, v.z, v.w }).ToArray());
        }

        public static void SetValues(this ComputeShader shader, string name, Texture value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"({nameof(name)}) is not valid");
                return;
            }

            shader.SetTexture(default, name, value);
        }

        // ------------------------------------------------
    }
}
