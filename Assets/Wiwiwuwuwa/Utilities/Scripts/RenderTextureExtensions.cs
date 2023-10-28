using UnityEngine;
using Unity.Mathematics;

namespace Wiwiwuwuwa.Utilities
{
    public static class RenderTextureExtensions
    {
        // ------------------------------------------------

        public static int3 GetSize(this RenderTexture renderTexture)
        {
            if (!renderTexture)
            {
                Debug.LogError($"({nameof(renderTexture)}) is not valid");
                return default;
            }

            return math.int3(renderTexture.width, renderTexture.height, renderTexture.volumeDepth);
        }

        // ------------------------------------------------
    }
}
