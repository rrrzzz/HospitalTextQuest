using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    public class NoiseFX : MonoBehaviour
    {

        [Range(0.0f, 5.0f)]
        public float grainIntensity = 0.2f;

        /// The size of the noise grains (1 = one pixel).
        [Range(0.1f, 50.0f)]
        public float grainSize = 2.0f;


        public Texture grainTexture;
        public Shader   shaderRGB;
        private Material m_Mat;

        protected void Start ()
        {
            if ( shaderRGB == null)
            {
                Debug.Log( "Noise shaders are not set up! Disabling noise effect." );
                enabled = false;
            }
            else
            {
                if ( !shaderRGB.isSupported ) // disable effect if RGB shader is not supported
                    enabled = false;
            }
        }

        protected Material material {
            get {
                if ( m_Mat == null ) {
                    m_Mat = new Material( shaderRGB );
                    m_Mat.hideFlags = HideFlags.HideAndDontSave;
                }
                
                return m_Mat;
            }
        }

        protected void OnDisable() {
            if ( m_Mat )
                DestroyImmediate( m_Mat );
        }

        private void SanitizeParameters()
        {
            grainIntensity = Mathf.Clamp( grainIntensity, 0.0f, 5.0f );
            grainSize = Mathf.Clamp( grainSize, 0.1f, 50.0f );
        }

        // Called by the camera to apply the image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination)
        {
            SanitizeParameters();

            Material mat = material;

            mat.SetTexture("_GrainTex", grainTexture);
            float grainScale = 1.0f / grainSize; // we have sanitized it earlier, won't be zero
            mat.SetVector("_GrainOffsetScale", new Vector4(
                                                   Random.value,
                                                   Random.value,
                                                   (float)Screen.width / (float)grainTexture.width * grainScale,
                                                   (float)Screen.height / (float)grainTexture.height * grainScale
                                                   ));
       
            mat.SetFloat("_Intensity", grainIntensity);
            Graphics.Blit (source, destination, mat);
        }
    }
}
