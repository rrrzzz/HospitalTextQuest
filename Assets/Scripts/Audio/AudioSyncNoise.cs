using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Audio
{
    public class AudioSyncNoise : AudioSyncer {

        public float noiseBeatScale;
        public float noiseRestScale;

        private NoiseFX _noiseScript;

        void Start(){
            _noiseScript = GetComponent<NoiseFX>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_isBeat) return;

            _noiseScript.grainIntensity = Mathf.Lerp(_noiseScript.grainIntensity, noiseRestScale, restSmoothTime * Time.deltaTime);
        }

        public override void OnBeat()
        {
            base.OnBeat();

            StopCoroutine("IncreaseNoise");
            StartCoroutine("IncreaseNoise", noiseBeatScale);
        }

        private IEnumerator IncreaseNoise(float target)
        {
            float initial = _noiseScript.grainIntensity;
            float timer = 0;

            while (Math.Abs(_noiseScript.grainIntensity - target) > 0.01f)
            {
                _noiseScript.grainIntensity = Mathf.Lerp(initial, target, timer / timeToBeat);
                timer += Time.deltaTime;

                yield return null;
            }

            m_isBeat = false;
        }
    }
}