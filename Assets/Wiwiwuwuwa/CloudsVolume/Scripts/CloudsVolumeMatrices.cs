using Unity.Mathematics;

namespace Wiwiwuwuwa.CloudsVolume
{
    public static class CloudsVolumeMatrices
    {
        // ----------------------------------------------------

        public static float4x4 GetDensityObjectToWorld(float cloudsAreaRange)
        {
            var matrix = float4x4.identity;

            matrix = math.mul(float4x4.Translate(-0.5f), matrix);
            matrix = math.mul(float4x4.Scale(cloudsAreaRange), matrix);

            return matrix;
        }

        public static float4x4 GetDensityWorldToObject(float cloudsAreaRange)
        {
            return math.inverse(GetDensityObjectToWorld(cloudsAreaRange));
        }

        public static float4x4 GetShadowsObjectToWorldMatrix(float cloudsLowerPlane, float cloudsUpperPlane, float cloudsAreaRange)
        {
            var matrix = float4x4.identity;

            matrix = math.mul(float4x4.Translate(-0.5f), matrix);
            matrix = math.mul(float4x4.Scale(math.float3(cloudsAreaRange, math.distance(cloudsLowerPlane, cloudsUpperPlane), cloudsAreaRange)), matrix);
            matrix = math.mul(float4x4.Translate(math.up() * math.lerp(cloudsLowerPlane, cloudsUpperPlane, 0.5f)), matrix);

            return matrix;
        }

        public static float4x4 GetShadowsWorldToObjectMatrix(float cloudsLowerPlane, float cloudsUpperPlane, float cloudsAreaRange)
        {
            return math.inverse(GetShadowsObjectToWorldMatrix(cloudsLowerPlane, cloudsUpperPlane, cloudsAreaRange));
        }

        // ----------------------------------------------------
    }
}
