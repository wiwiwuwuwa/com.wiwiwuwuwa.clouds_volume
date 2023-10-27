Shader "Wiwiwuwuwa/Clouds Volume/Skybox"
{
    Properties
    {
        [NoScaleOffset]
        _SkyboxTexture ("Skybox Texture", Cube) = "black" {}
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
                const float4 cloudColor = float4(1.0, 1.0, 1.0, cloudDelta.r);

                FragToData output = (FragToData) 0.0;
                output.Color = lerp(output.Color, cloudColor.rgb, cloudColor.a);
                return output;
            }

            ENDHLSL

            // --------------------------------------------
        }
    }
}
