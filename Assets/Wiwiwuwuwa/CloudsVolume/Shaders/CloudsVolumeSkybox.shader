Shader "Wiwiwuwuwa/CloudsVolume/Skybox"
{
    Properties
    {
        [NoScaleOffset]
        _SkyboxTexture ("Skybox Texture", Cube) = "black" {}

        [HDR]
        _SkyGradientLower ("Sky Gradient Lower", Color) = (0.455, 0.529, 0.604, 1.000)

        [HDR]
        _SkyGradientUpper ("Sky Gradient Upper", Color) = (0.078, 0.109, 0.200, 1.000)

        [HDR]
        _CloudsAmbient ("Clouds Ambient", Color) = (0.039, 0.054, 0.100, 1.000)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }

        Pass
        {
            Name "Skybox"

            Cull Off
            ZClip Off
            ZTest LEqual
            ZWrite Off

            // --------------------------------------------

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            // --------------------------------------------

            #include "UnityCG.cginc"
            #include "../../Utilities/Shaders/Library/GetFadeGradient.hlsl"
            #include "../../Utilities/Shaders/Library/GetFogExp2.hlsl"
            #include "../../Utilities/Shaders/Library/GetRemap.hlsl"

            // --------------------------------------------

            struct DataToVert
            {
                float3 PosOS : POSITION;
            };

            struct VertToFrag
            {
                float3 DirWS : TEXCOORD;
                float4 PosCS : SV_POSITION;
            };

            struct FragToData
            {
                float3 Color : SV_TARGET;
            };

            // --------------------------------------------

            UNITY_DECLARE_TEXCUBE(_SkyboxTexture);

            float4 _SunCol;

            float4 _SunDir;

            float3 _SkyGradientLower;

            float3 _SkyGradientUpper;

            float3 _CloudsAmbient;

            // --------------------------------------------

            VertToFrag Vert(DataToVert input)
            {
                VertToFrag output = (VertToFrag)0;
                output.DirWS = normalize(input.PosOS);
                output.PosCS = UnityObjectToClipPos(input.PosOS);
                return output;
            }

            FragToData Frag(VertToFrag input)
            {
                input.DirWS = normalize(input.DirWS);

                const float1 airDelta = Wiwiw_GetFogExp2(2.5, Wiwiw_GetRemap(input.DirWS.y, -1.0, 1.0, 1.0, 0.0));
                const float3 airColor = lerp(_SkyGradientLower, _SkyGradientUpper, airDelta.x);

                const float1 sunDelta = Wiwiw_GetFadeGradient(Wiwiw_GetRemap(dot(input.DirWS, _SunDir.xyz), 0.9975, 1.0, 1.0, 0.0), 0.5, 1.0, 1.0, 0.0);
                const float4 sunColor = float4(_SunCol.xyz, sunDelta.x);

                const float4 cloudDelta = UNITY_SAMPLE_TEXCUBE(_SkyboxTexture, input.DirWS);
                const float4 cloudColor = float4(_CloudsAmbient + cloudDelta.r * _SunCol.xyz, cloudDelta.g);
                const float4 sunbmColor = float4(_SunCol.xyz, cloudDelta.b);

                FragToData output = (FragToData)0;
                output.Color = airColor;
                output.Color = lerp(output.Color, sunColor.rgb, sunColor.a);
                output.Color = lerp(output.Color, cloudColor.rgb, cloudColor.a);
                output.Color = lerp(output.Color, sunbmColor.rgb, sunbmColor.a);
                return output;
            }

            ENDHLSL

            // --------------------------------------------
        }
    }
}
