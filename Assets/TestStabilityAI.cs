using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Wiwiwuwuwa.CloudsVolume;
using Wiwiwuwuwa.Utilities;
using System;
using UnityEditor.Scripting.Python;

public class TestStabilityAI : MonoBehaviour
{
    // ----------------------------------------------------

    [SerializeField]
    Material skybox = default;

    // --------------------------------

    void OnEnable()
    {
        var spheremap = new RenderTexture(new RenderTextureDescriptor
        {
            dimension = TextureDimension.Tex2D,
            colorFormat = RenderTextureFormat.ARGB32,
            width = 1024,
            height = 1024,
            volumeDepth = 1,
            bindMS = false,
            msaaSamples = 1,
            enableRandomWrite = true,
        })
        {
            name = "Stability AI Spheremap",
        };

        skybox.SetTexture("_StabilityTexture", spheremap);
        skybox.SetFloat("_TextureBlending", 0f);
    }

    void OnDisable()
    {
        DestroyImmediate(skybox.GetTexture("_StabilityTexture"));

        skybox.SetTexture("_StabilityTexture", default);
        skybox.SetFloat("_TextureBlending", 0f);

    }

    // --------------------------------

    [ContextMenu("Apply Stability AI")]
    void ApplyStabilityAI()
    {
        EditorUtility.DisplayProgressBar("Applying Stability AI", "Converting cubemap to spheremap...", 0f);

        var cubemap = skybox.GetTexture("_SkyboxTexture") as RenderTexture;
        var spheremap = skybox.GetTexture("_StabilityTexture") as RenderTexture;

        var convertOperation = new ConvertCubemapToSpheremap(cubemap, spheremap);
        while (convertOperation.MoveNext()) { }

        var litOperation = new LitSky(skybox);
        while (litOperation.MoveNext()) { }

        TransformRenderTexture(ref spheremap);
        skybox.SetFloat("_TextureBlending", 1f);

        EditorUtility.ClearProgressBar();
    }

    [ContextMenu("Clear Stability AI")]
    void ClearStabilityAI()
    {
        skybox.SetFloat("_TextureBlending", 0f);
    }

    // --------------------------------

    public void TransformRenderTexture(ref RenderTexture renderTexture)
    {
        // Conversion of RenderTexture to Texture2D
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null; // Remove the active RenderTexture

        // Encoding Texture2D to a PNG, then to a Base64 string
        string base64String = Convert.ToBase64String(texture2D.EncodeToPNG());

        // Check if the Base64 string is valid
        if (!IsBase64String(base64String))
        {
            Debug.LogError("Invalid Base64 string");
            Destroy(texture2D);
            return;
        }

        // Clean up the Texture2D and RenderTexture
        Destroy(texture2D);

        // Transform the Base64 string using the TransformImageBase64 function
        string transformedBase64 = TransformImageBase64(base64String);

        // Check if the transformed Base64 string is valid
        if (!IsBase64String(transformedBase64))
        {
            Debug.LogError("Invalid transformed Base64 string");
            return;
        }

        // Convert the transformed Base64 back to a byte array
        byte[] transformedBytes = Convert.FromBase64String(transformedBase64);

        // Load the transformed image into the existing texture2D
        if (texture2D.LoadImage(transformedBytes))
        {
            // If LoadImage succeeds, update the RenderTexture
            RenderTexture.active = renderTexture;
            Graphics.Blit(texture2D, renderTexture);
            RenderTexture.active = null;
        }

        // Clean up the Texture2D
        Destroy(texture2D);
    }

    private bool IsBase64String(string base64String)
    {
        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    string TransformImageBase64(string imageBase64)
    {
        // PLEASE INSERT YOUR API KEY INSIDE THE PYTHON CODE BELOW

        var transformedImageBase64 = string.Empty;

        var listener = (Application.LogCallback)((logString, stackTrace, type) =>
        {
            if (type == LogType.Log)
            {
                transformedImageBase64 = logString;
            }
        });

        Application.logMessageReceived += listener;

        var pythonCode =
$@"
imageBase64 = '{imageBase64}'
"
+
@"
import base64
import requests
import UnityEngine

def transform_image(init_image_base64):
    UnityEngine.Debug.Log('Transforming image...')

    # A function to transform the given image using an AI model and return a new image.

    # Convert base64 string to bytes
    init_image_bytes = base64.b64decode(init_image_base64)

    # Your unique API key for the service
    api_key = '{INSERT_API_KEY_HERE}'

    # Request to the API for image transformation
    response = requests.post(
        'https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/image-to-image',
        headers={
            'Accept': 'application/json',
            'Authorization': f'Bearer {api_key}'
        },
        files={
            'init_image': ('image.png', init_image_bytes)
        },
        data={
            'init_image_mode': 'IMAGE_STRENGTH',
            'image_strength': 0.85,
            'steps': 40,
#           'width': 1024,
#           'height': 1024,
            'seed': 123124677,
            'cfg_scale': 5,
            'samples': 1,
            'text_prompts[0][text]': 'sky with clouds fisheye photo, cinematic, sun, contrast, sharp, clear, beautiful, artistic, colorful, vibrant, vivid, high quality, retro, vintage',
            'text_prompts[0][weight]': 1,
            'text_prompts[1][text]': 'blurry, bad',
            'text_prompts[1][weight]': -1,
        }
    )

    # Raise an exception if the request was not successful
    if response.status_code != 200:
        UnityEngine.Debug.LogError(f'Error: {response.status_code} {response.reason} {response.text}')
        return

    # Parse the response to get the transformed image in base64 format
    data = response.json()
    transformed_image_base64 = data['artifacts'][0]['base64']

    # Return the transformed image as a base64 string
    UnityEngine.Debug.Log(transformed_image_base64)

transform_image(imageBase64)
";

        PythonRunner.RunString(pythonCode);

        Application.logMessageReceived -= listener;

        return transformedImageBase64;
    }

    // ----------------------------------------------------
}
