using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Wiwiwuwuwa.Utilities
{
    public class ConvertCubemapToSpheremap : ComputeOperation
    {
        // ------------------------------------------------

        const string SHADER_CUBEMAP_TEXTURE_PROPERTY = "_CubemapTexture";

        const string SHADER_SPHEREMAP_TEXTURE_PROPERTY = "_SpheremapTexture";

        // ----------------------------

        readonly RenderTexture cubemapTexture = default;

        readonly RenderTexture spheremapTexture = default;

        // ----------------------------

        public ConvertCubemapToSpheremap(RenderTexture cubemapTexture, RenderTexture spheremapTexture)
        {
            this.cubemapTexture = cubemapTexture;
            this.spheremapTexture = spheremapTexture;
        }

        protected override IEnumerator Execute()
        {
            if (!cubemapTexture)
            {
                Debug.LogError($"({nameof(cubemapTexture)}) is not valid");
                yield break;
            }

            if (!spheremapTexture)
            {
                Debug.LogError($"({nameof(spheremapTexture)}) is not valid");
                yield break;
            }

            var computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Wiwiwuwuwa/Utilities/Shaders/CubemapToSpheremap.compute");
            if (!computeShader)
            {
                Debug.LogError($"({nameof(computeShader)}) is not valid");
                yield break;
            }

            computeShader.SetValues(SHADER_CUBEMAP_TEXTURE_PROPERTY, cubemapTexture);
            computeShader.SetValues(SHADER_SPHEREMAP_TEXTURE_PROPERTY, spheremapTexture);

            var dispatchOperation = new DispatchComputeShader(computeShader, spheremapTexture.GetSize());
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
