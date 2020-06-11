using System;
using UnityEngine;

namespace Shaders.Glitch
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class Jitter : MonoBehaviour
    {
        [Range(0, 2)]
        public float jitter;

        [SerializeField] Shader _shader;
        Material _material;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            var isJitterZero = Math.Abs(jitter) < 0.001f;
            if (!isJitterZero)
            {
                var sl_thresh = Mathf.Clamp01(1.0f - jitter * 1.2f);
                var sl_disp = 0.002f + Mathf.Pow(jitter, 3) * 0.05f;
                _material.SetVector("_Jitter", new Vector2(sl_disp, sl_thresh));
            }
            Graphics.Blit(source, destination, _material);
        }
    }
}