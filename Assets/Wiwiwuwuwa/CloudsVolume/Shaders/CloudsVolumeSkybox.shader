Shader "Wiwiwuwuwa/Clouds Volume/Skybox"
{
    Properties
    {
        [NoScaleOffset]
        _SkyboxTexture ("Skybox Texture", Cube) = "black" {}

        _BentNormalScale ("Bent Normal Scale", Float) = 128.0

        _BentNormalPower ("Bent Normal Power", Float) = 12.0

        _GradientPoint0 ("Gradient Point 0", Float) = 0.0

        _GradientValue0 ("Gradient Value 0", Color) = (0.03921569, 0.1254902, 0.2666667, 1.0) // #0A2044

        _GradientPoint1 ("Gradient Point 1", Float) = 0.5

        _GradientValue1 ("Gradient Value 1", Color) = (0.03137255, 0.2901961, 0.7450981, 1.0) // #084ABE

        _GradientPoint2 ("Gradient Point 2", Float) = 1.0

        _GradientValue2 ("Gradient Value 2", Color) = (0.0, 0.2313726, 0.7372549, 1.0) // #003DBC

        _AmbientPoint0 ("Ambient Point 0", Float) = 0.00

        _AmbientValue0 ("Ambient Value 0", Color) = (0.0, 0.0, 0.0, 1.0) // #000000

        _AmbientPoint1 ("Ambient Point 1", Float) = 0.25

        _AmbientValue1 ("Ambient Value 1", Color) = (0.03921569, 0.1254902, 0.2666667, 1.0) // #0A2044
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

            float _BentNormalScale;

            float _BentNormalPower;

            float _GradientPoint0;

            float3 _GradientValue0;

            float _GradientPoint1;

            float3 _GradientValue1;

            float _GradientPoint2;

            float3 _GradientValue2;

            float _AmbientPoint0;

            float3 _AmbientValue0;

            float _AmbientPoint1;

            float3 _AmbientValue1;

            float3 _SunDir;

            float3 _SunCol;

            // --------------------------------------------

            float3 GetGradientColor(float3 dirWS)
            {
                float3 gradientColor = 0.0;

                const bool isInInterval0 = dirWS.y < _GradientPoint1;
                const float3 colorOfInterval0 = lerp(_GradientValue0, _GradientValue1, smoothstep(_GradientPoint0, _GradientPoint1, dirWS.y));
                gradientColor += isInInterval0 ? colorOfInterval0 : 0.0;

                const bool isInInterval1 = dirWS.y >= _GradientPoint1;
                const float3 colorOfInterval1 = lerp(_GradientValue1, _GradientValue2, smoothstep(_GradientPoint1, _GradientPoint2, dirWS.y));
                gradientColor += isInInterval1 ? colorOfInterval1 : 0.0;

                return gradientColor;
            }

            float3 GetAmbientColor(float3 dirWS)
            {
                return lerp(_AmbientValue0, _AmbientValue1, smoothstep(_AmbientPoint0, _AmbientPoint1, dirWS.y));
            }

            // --------------------------------------------

            float4 GetCloudsColor(float3 dirWS)
            {
                const float4 cloudsData = UNITY_SAMPLE_TEXCUBE(_SkyboxTexture, dirWS);

                const float3 cloudsNormal = mad(cloudsData.rgb, 2.0, -1.0);
                const float cloudsAlpha = cloudsData.a;

                float3 cloudsColor = 0.0;

                float cloudsSunFactor = 0.0;
                cloudsSunFactor = dot(cloudsNormal, -_SunDir);
                cloudsSunFactor = mad(cloudsSunFactor, 0.5, 0.5);
                cloudsSunFactor = _BentNormalScale * pow(cloudsSunFactor, _BentNormalPower);
                cloudsSunFactor = cloudsSunFactor * rcp(cloudsSunFactor + 1.0);

                cloudsColor += cloudsSunFactor * _SunCol;

                float cloudsSkyFactor = 0.0;
                cloudsSkyFactor = dot(cloudsNormal, float3(0.0, 1.0, 0.0));
                cloudsSkyFactor = mad(cloudsSkyFactor, 0.5, 0.5);

                cloudsColor += GetAmbientColor(cloudsSkyFactor);

                return float4(cloudsColor, cloudsAlpha);
            }

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
                const float4 cloudsColor = GetCloudsColor(input.DirWS);

                FragToData output = (FragToData)0.0;
                output.Color = GetGradientColor(input.DirWS);
                output.Color = lerp(output.Color, cloudsColor.rgb, cloudsColor.a);
                return output;
            }

            ENDHLSL

            // --------------------------------------------
        }
    }
}
