using System;
using UnityEngine;
using UnityEditor;

namespace Wiwiwuwuwa.CloudsVolume
{
    [CreateAssetMenu(fileName = "CloudsVolumeGlobalSettings", menuName = "Wiwiwuwuwa/Clouds Volume/Global Settings")]
    public class CloudsVolumeGlobalSettings : ScriptableObject
    {
        // ------------------------------------------------

        [Header("Cubemap Settings")]

        [SerializeField]
        ComputeShader cubemapShader = default;

        [Header("Cookies Settings")]

        [SerializeField]
        ComputeShader cookiesShader = default;

        // --------------------------------

        public event Action OnValidateEvent = default;

        public ComputeShader CubemapShader => cubemapShader;

        public ComputeShader CookiesShader => cookiesShader;

        // --------------------------------

        void OnValidate()
        {
            OnValidateEvent?.Invoke();
        }

        // ------------------------------------------------
    }
}
