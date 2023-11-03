using System.Collections;
using UnityEngine;
using UnityEditor;
using Wiwiwuwuwa.Utilities;

namespace Wiwiwuwuwa.CloudsVolume
{
    public class LitSky : ComputeOperation
    {
        // ------------------------------------------------

        readonly Material material = default;

        // ----------------------------

        public LitSky(Material material)
        {
            this.material = material;
        }

        protected override IEnumerator Execute()
        {
            if (!material)
            {
                Debug.LogError($"({nameof(material)}) is not valid");
                yield break;
            }

            var computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Wiwiwuwuwa/CloudsVolume/Shaders/LitSky.compute");
            if (!computeShader)
            {
                Debug.LogError($"({nameof(computeShader)}) is not valid");
                yield break;
            }

            computeShader.SetValues("_MainTexture", material.GetTexture("_StabilityTexture"));
            computeShader.SetValues("_SkyboxTexture", material.GetTexture("_SkyboxTexture"));
            computeShader.SetValues("_StabilityTexture", material.GetTexture("_StabilityTexture"));
            computeShader.SetValues("_TextureBlending", material.GetFloat("_TextureBlending"));
            computeShader.SetValues("_BentNormalScale", material.GetFloat("_BentNormalScale"));
            computeShader.SetValues("_BentNormalPower", material.GetFloat("_BentNormalPower"));
            computeShader.SetValues("_GradientPoint0", material.GetFloat("_GradientPoint0"));
            computeShader.SetValues("_GradientValue0", material.GetVector("_GradientValue0"));
            computeShader.SetValues("_GradientPoint1", material.GetFloat("_GradientPoint1"));
            computeShader.SetValues("_GradientValue1", material.GetVector("_GradientValue1"));
            computeShader.SetValues("_GradientPoint2", material.GetFloat("_GradientPoint2"));
            computeShader.SetValues("_GradientValue2", material.GetVector("_GradientValue2"));
            computeShader.SetValues("_GradientPoint3", material.GetFloat("_GradientPoint3"));
            computeShader.SetValues("_GradientValue3", material.GetVector("_GradientValue3"));
            computeShader.SetValues("_AmbientPoint0", material.GetFloat("_AmbientPoint0"));
            computeShader.SetValues("_AmbientValue0", material.GetVector("_AmbientValue0"));
            computeShader.SetValues("_AmbientPoint1", material.GetFloat("_AmbientPoint1"));
            computeShader.SetValues("_AmbientValue1", material.GetVector("_AmbientValue1"));
            computeShader.SetValues("_SunDir", material.GetVector("_SunDir"));
            computeShader.SetValues("_SunCol", material.GetVector("_SunCol"));

            var dispatchOperation = new DispatchComputeShader(computeShader, (material.GetTexture("_StabilityTexture") as RenderTexture).GetSize());
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
