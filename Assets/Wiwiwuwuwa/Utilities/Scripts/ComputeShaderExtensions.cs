using UnityEngine;
using Unity.Mathematics;

namespace Wiwiwuwuwa.Utilities
{
    public static class ComputeShaderExtensions
    {
        public static void SetFloats(this ComputeShader shader, string name, float2 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, 0f, 0f, 0f, value.y, 0f, 0f, 0f);
        }

        public static void SetFloats(this ComputeShader shader, string name, float3 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, 0f, 0f, 0f, value.y, 0f, 0f, 0f, value.z, 0f, 0f, 0f);
        }

        public static void SetFloats(this ComputeShader shader, string name, float4 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, 0f, 0f, 0f, value.y, 0f, 0f, 0f, value.z, 0f, 0f, 0f, value.w, 0f, 0f, 0f);
        }

        public static void SetVector(this ComputeShader shader, string name, float2 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y);
        }

        public static void SetVector(this ComputeShader shader, string name, float3 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y, value.z);
        }

        public static void SetVector(this ComputeShader shader, string name, float4 value)
        {
            if (!shader)
            {
                Debug.LogError($"({nameof(shader)}) is not valid");
                return;
            }

            shader.SetFloats(name, value.x, value.y, value.z, value.w);
        }
    }
}
