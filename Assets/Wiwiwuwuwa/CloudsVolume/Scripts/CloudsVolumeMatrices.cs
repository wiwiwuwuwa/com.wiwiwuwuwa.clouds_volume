using UnityEngine;
using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    public static class ClodusVolumeMatrices
    {
        // ----------------------------------------------------

        public static float4x4 GetShadowsToDensityMatrix(float shadowsAreaScale)
        {
            return math.mul(GetShadowsWorldToObjectMatrix(shadowsAreaScale), GetDensityObjectToWorldMatrix());
        }

        public static float4x4 GetDensityObjectToWorldMatrix()
        {
            var trans = float4x4.Translate(math.float3(-0.5f, 0.0f, -0.5f));
            var scale = float4x4.Scale(math.float3(2.0f, 1.0f, 2.0f));
            return math.mul(scale, trans);
        }

        public static float4x4 GetDensityWorldToObjectMatrix()
        {
            return math.inverse(GetDensityObjectToWorldMatrix());
        }

        public static float4x4 GetShadowsObjectToWorldMatrix(float areaScale)
        {
            var trans = float4x4.Translate(-0.5f);
            var rotat = CloudsVolumeEnvironment.GetSunObjectToWorldMatrix();
            var scale = float4x4.Scale(2.0f * areaScale);
            return math.mul(rotat, math.mul(scale, trans));
        }

        public static float4x4 GetShadowsWorldToObjectMatrix(float areaScale)
        {
            return math.inverse(GetShadowsObjectToWorldMatrix(areaScale));
        }

        // ----------------------------------------------------
    }
}
