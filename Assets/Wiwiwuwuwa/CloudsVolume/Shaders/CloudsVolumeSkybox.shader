Shader "Wiwiwuwuwa/Clouds Volume/Skybox"
{
    Properties
    {
        [Header(Textures)]
        [Space]

        _SkyboxTexture ("Skybox Texture", 2DArray) = "black" {}

        _StabilityTexture ("Stability Texture", 2D) = "black" {}

        _TextureBlending ("Texture Blending", Float) = 0.0

        [Header(Bent Normal Params)]
        [Space]

        _BentNormalScale ("Bent Normal Scale", Float) = 20.0

        _BentNormalPower ("Bent Normal Power", Float) = 10.0

        [Header(Gradient Params)]
        [Space]

        _GradientPoint0 ("Gradient Point 0", Float) = -0.05

        _GradientValue0 ("Gradient Value 0", Color) = (0.3568628, 0.2745098, 0.3254902, 1.0) // #5B4653

        _GradientPoint1 ("Gradient Point 1", Float) = 0.0

        _GradientValue1 ("Gradient Value 1", Color) = (0.8509804, 0.7215686, 0.682353, 1.0) // #D9B8AE

        _GradientPoint2 ("Gradient Point 2", Float) = 0.5

        _GradientValue2 ("Gradient Value 2", Color) = (0.4509804, 0.6000001, 0.7490196, 1.0) // #7399BF

        _GradientPoint3 ("Gradient Point 3", Float) = 1.0

        _GradientValue3 ("Gradient Value 3", Color) = (0.3803922, 0.3490196, 0.6980392, 1.0) // #6159B2

        [Header(Ambient Params)]
        [Space]

        _AmbientPoint0 ("Ambient Point 0", Float) = 0.0

        _AmbientValue0 ("Ambient Value 0", Color) = (0.1490196, 0.1607843, 0.2980392, 1.0) // #26294C

        _AmbientPoint1 ("Ambient Point 1", Float) = 1.0

        _AmbientValue1 ("Ambient Value 1", Color) = (0.4, 0.3568628, 0.3411765, 1.0) // #665B57
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
            #include "Library/LibSky.hlsl"

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

            VertToFrag Vert(DataToVert input)
            {
                VertToFrag output = (VertToFrag)0;
                output.DirWS = normalize(input.PosOS);
                output.PosCS = UnityObjectToClipPos(input.PosOS);
                return output;
            }

            FragToData Frag(VertToFrag input)
            {
                FragToData output = (FragToData)0.0;
                output.Color = GetSkyboxColor(normalize(input.DirWS));
                return output;
            }

            ENDHLSL

            // --------------------------------------------
        }
    }
}
