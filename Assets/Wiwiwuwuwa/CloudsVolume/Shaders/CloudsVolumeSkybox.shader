Shader "Wiwiwuwuwa/Clouds Volume/Skybox"
{
    Properties
    {
        [NoScaleOffset]
        _SkyboxTexture ("Skybox Texture", Cube) = "black" {}

        // sky color param
        _SkyColor ("Sky Color", Color) = (0.529, 0.808, 0.922, 1.0)

        // horizon color param
        _HorizonColor ("Horizon Color", Color) = (0.0, 0.0, 0.0, 1.0)

        // sun color param
        _SunColor ("Sun Color", Color) = (1.474, 0.986, 0.643, 1.0)

        // ambient color param
        _AmbientColor ("Ambient Color", Color) = (0.0, 0.0, 0.0, 1.0)
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

            float3 _SkyColor;

            float3 _HorizonColor;

            float3 _SunColor;

            float3 _AmbientColor;

            float3 GetSkyColor(float3 dirWS)
            {
                    const float3 skyColor = _SkyColor;
                    const float3 horizonColor = _HorizonColor;
                    return lerp(skyColor, horizonColor, pow(saturate(dirWS.y), 1));
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

                const float4 cloudDelta = UNITY_SAMPLE_TEXCUBE(_SkyboxTexture, input.DirWS);

                const float3 sunDir = normalize(float3(0.2126, 0.7152, 0.0722));
                const float3 cldDir = mad(cloudDelta.rgb, 2.0, -1.0);

                float cloudLightness = pow(mad(dot(cldDir, sunDir), 0.5, 0.5), 20.0) * 100000.0;
                // reinhard
                cloudLightness = 2.0 * cloudLightness / (cloudLightness + 1.0);

                const float4 skyColor = float4(GetSkyColor(input.DirWS), 1.0);
                const float4 cloudColor = float4(_SunColor * cloudLightness + _AmbientColor * length(cldDir), cloudDelta.a);

                FragToData output = (FragToData) 0.0;
                output.Color = lerp(output.Color, skyColor.rgb, skyColor.a);
                output.Color = lerp(output.Color, cloudColor.rgb, cloudColor.a);
                return output;
            }

            ENDHLSL

            // --------------------------------------------
        }
    }
}
