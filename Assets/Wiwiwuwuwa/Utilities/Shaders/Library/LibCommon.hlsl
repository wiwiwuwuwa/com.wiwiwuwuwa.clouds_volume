#ifndef WIWIW_LIB_COMMON_INCLUDED
#define WIWIW_LIB_COMMON_INCLUDED

// --------------------------------------------------------
// Math
// --------------------------------------------------------

static const float WIWIW_PI = 3.1415926535897932384626433832795;

// --------------------------------------------------------

float Wiwiw_BlendOverlay(in float inColorA, in float inColorB)
{
    return (inColorA < 0.5) ? (2.0 * inColorA * inColorB) : (1.0 - 2.0 * (1.0 - inColorA) * (1.0 - inColorB));
}

uint3 Wiwiw_Get3DFrom1D(in uint inCoord, in uint inWidth, in uint inHeight)
{
    uint3 outCoord = 0;
	outCoord.x = inCoord % inWidth;
	outCoord.y = (inCoord / inWidth) % inHeight;
	outCoord.z = inCoord / (inWidth * inHeight);
	return outCoord;
}

float Wiwiw_GetNan()
{
    return asfloat(0xFFFFFFFF);
}

float3 Wiwiw_GetPointOnCubemap(in float2 inSideCoord, in uint inSideIndex)
{
    switch (inSideIndex)
    {
        case 0: return float3(1.0, inSideCoord.y, 1.0 - inSideCoord.x); // +x
        case 1: return float3(0.0, inSideCoord.y, inSideCoord.x); // -x
        case 2: return float3(inSideCoord.x, 1.0, 1.0 - inSideCoord.y); // +y
        case 3: return float3(inSideCoord.x, 0.0, inSideCoord.y); // -y
        case 4: return float3(inSideCoord.x, inSideCoord.y, 1.0); // +z
        case 5: return float3(1.0 - inSideCoord.x, inSideCoord.y, 0.0); // -z
        default: return 0.5;
    }
}

float3 Wiwiw_GetRayIntersectPlaneY(in float3 inRayPos, in float3 inRayDir, in float inPlaneY)
{
	const float isValid = (inRayDir.y != 0.0);

	const float t = (inPlaneY - inRayPos.y) * rcp(inRayDir.y);
	const float3 intersectPos = inRayPos + t * inRayDir;

	return isValid ? intersectPos : Wiwiw_GetNan();
}

float Wiwiw_GetRayTime(in float3 inRayPos, in float3 inRayDir, in float3 inPos)
{
    return dot(inPos - inRayPos, inRayDir) * rcp(dot(inRayDir, inRayDir));
}

float Wiwiw_InverseLerp(in float inMin, in float inMax, in float inValue)
{
	return (inValue - inMin) * rcp(inMax - inMin);
}

float2 Wiwiw_InverseLerp(in float2 inMin, in float2 inMax, in float2 inValue)
{
    return (inValue - inMin) * rcp(inMax - inMin);
}

float3 Wiwiw_InverseLerp(in float3 inMin, in float3 inMax, in float3 inValue)
{
    return (inValue - inMin) * rcp(inMax - inMin);
}

float4 Wiwiw_InverseLerp(in float4 inMin, in float4 inMax, in float4 inValue)
{
    return (inValue - inMin) * rcp(inMax - inMin);
}

bool Wiwiw_IsNan(in float inValue)
{
    return any(inValue == Wiwiw_GetNan());
}

bool Wiwiw_IsNan(in float3 inValue)
{
	return any(inValue == Wiwiw_GetNan());
}

float Wiwiw_Remap(in float inX, in float inOldMin, in float inOldMax, in float inNewMin, in float inNewMax)
{
    return lerp(inNewMin, inNewMax, Wiwiw_InverseLerp(inOldMin, inOldMax, inX));
}

float2 Wiwiw_Remap(in float2 inX, in float2 inOldMin, in float2 inOldMax, in float2 inNewMin, in float2 inNewMax)
{
    return lerp(inNewMin, inNewMax, Wiwiw_InverseLerp(inOldMin, inOldMax, inX));
}

float3 Wiwiw_Remap(in float3 inX, in float3 inOldMin, in float3 inOldMax, in float3 inNewMin, in float3 inNewMax)
{
    return lerp(inNewMin, inNewMax, Wiwiw_InverseLerp(inOldMin, inOldMax, inX));
}

float4 Wiwiw_Remap(in float4 inX, in float4 inOldMin, in float4 inOldMax, in float4 inNewMin, in float4 inNewMax)
{
    return lerp(inNewMin, inNewMax, Wiwiw_InverseLerp(inOldMin, inOldMax, inX));
}

float3 Wiwiw_SpheremapToCubemapUV(in float2 inCoord)
{
    const float2 remappedCoord = Wiwiw_Remap(inCoord, 0.0, 1.0, -1.0, 1.0);
    const float theta = atan2(remappedCoord.y, remappedCoord.x);
    const float radius = length(remappedCoord);
    const float phi = radius * WIWIW_PI * 0.5;
    const float sinPhi = sin(phi);

    float3 dir3D = 0.0;
    dir3D.x = sinPhi * cos(theta);
    dir3D.y = cos(phi);
    dir3D.z = sinPhi * sin(theta);
    return dir3D;
}

float2 Wiwiw_CubemapToSpheremapUV(in float3 dir3D)
{
    const float phi = acos(dir3D.y);
    const float theta = atan2(dir3D.z, dir3D.x);
    const float radius = phi * rcp(WIWIW_PI * 0.5);

    float2 remappedCoord = 0.0;
    remappedCoord.x = radius * cos(theta);
    remappedCoord.y = radius * sin(theta);
    remappedCoord = Wiwiw_Remap(remappedCoord, -1.0, 1.0, 0.0, 1.0);
    return remappedCoord;
}

float4 Wiwiw_SampleCubemap(in Texture2DArray inCubemap, in SamplerState inSampler, in float3 inDir)
{
    const float absX = abs(inDir.x);
    const float absY = abs(inDir.y);
    const float absZ = abs(inDir.z);

    float2 uv = 0.0;
    float id = 0.0;

    const bool isAxisX = (absX >= absY && absX >= absZ);
    uv = isAxisX ? inDir.zy * rcp(absX) : uv;
    id = isAxisX ? (inDir.x >= 0.0 ? 0.0 : 1.0) : id;

    const bool isAxisY = (absY >= absX && absY >= absZ);
    uv = isAxisY ? inDir.xz * rcp(absY) : uv;
    id = isAxisY ? (inDir.y >= 0.0 ? 2.0 : 3.0) : id;

    const bool isAxisZ = (absZ >= absX && absZ >= absY);
    uv = isAxisZ ? inDir.xy * rcp(absZ) : uv;
    id = isAxisZ ? (inDir.z >= 0.0 ? 4.0 : 5.0) : id;

    uv.x = (id == 0.0 || id == 5.0) ? -uv.x : uv.x;
    uv.y = (id == 2.0) ? -uv.y : uv.y;

    uv = Wiwiw_Remap(uv, -1.0, 1.0, 0.0, 1.0);
    return inCubemap.SampleLevel(inSampler, float3(uv, id), 0.0);
}

float4 Wiwiw_SampleSpheremap(in Texture2D inSpheremap, in SamplerState inSampler, in float3 inDir)
{
    return inSpheremap.SampleLevel(inSampler, Wiwiw_CubemapToSpheremapUV(inDir), 0.0);
}

// --------------------------------------------------------

#endif // WIWIW_LIB_COMMON_INCLUDED
