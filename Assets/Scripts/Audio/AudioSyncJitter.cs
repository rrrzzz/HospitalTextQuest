using System;
using System.Collections;
using Shaders.Glitch;
using UnityEngine;

namespace Audio
{
    public class AudioSyncJitter : AudioSyncer {

        public float glitchBeatScale = 0.2f;
        public float glitchRestScale;

        private Jitter _glitchScript;

        void Start(){
            _glitchScript = Camera.main.GetComponent<Jitter>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_isBeat) return;

            _glitchScript.jitter = Mathf.Lerp(_glitchScript.jitter, glitchRestScale, restSmoothTime * Time.deltaTime);
        }

        public override void OnBeat()
        {
            base.OnBeat();

            StopCoroutine("IncreaseJitter");
            StartCoroutine("IncreaseJitter", glitchBeatScale);
        }

        private IEnumerator IncreaseJitter(float target)
        {
            float initial = _glitchScript.jitter;
            float timer = 0;

            while (Math.Abs(_glitchScript.jitter - target) > 0.01f)
            {
                _glitchScript.jitter = Mathf.Lerp(initial, target, timer / timeToBeat);
                timer += Time.deltaTime;

                yield return null;
            }

            m_isBeat = false;
        }
    }
}