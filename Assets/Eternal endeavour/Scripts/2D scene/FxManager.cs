using System;
using System.Collections;
using Audio;
using Shaders.Glitch;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace _2D_scene
{
    public class FxManager : MonoBehaviour
    {
        private const char ParamsSeparator = ';';
        private const char TagValueSeparator = ':';

        private const string JitterTag = "jit";
        private const string JitterBeatTag = "jitBeat";
        private const string JitterRestTag = "jitRest";
        private const string JitterSyncTag = "sync";
        private const string NoiseTag = "noise";
        private const string SyncOn = "on";
        private const string TimeToChangeTag = "timeToChange";
        private const string HeartRateTag = "heartRate";
        private const string MusicTag = "music";
        private const string PauseTextTag = "pauseText";
        
        private const float JitterDef = 0f;
        private const float JitterBeatDef = 0.2f;
        private const float JitterRestDef = 0;
        private const float NoiseDef = 0.3f;
        private const float TimeToChangeDef = 0.5f;
        private const int BpmDef = 70;
    
        public SoundManager soundManager;
        public float timeToChange = 0.5f;

        private Jitter _jitter;
        private NoiseFX _noise;
        private AudioSyncJitter _audioJitter;
        
        //add interpolation when effects change
    
        // Start is called before the first frame update
        void Start()
        {
            TextPlayback.TagEncountered += ParseTagText;
            InitializeScriptObjects();
        }

        private void InitializeScriptObjects()
        {
            var cam = Camera.main;
            _jitter = cam.GetComponent<Jitter>();
            _noise = cam.GetComponent<NoiseFX>();
            _audioJitter = GetComponent<AudioSyncJitter>();
        }


        void ParseTagText(object obj, string tagText)
        {
            var paramsText = tagText.Split(ParamsSeparator);

            foreach (var param in paramsText)
            {
                var parsed = param.Split(TagValueSeparator);
                var tag = parsed[0].Trim();
                var value = parsed[1].Trim();
                ParseParam(tag, value, (TextPlayback)obj);
            }
        }

        //TODO: Write cases for other params
        void ParseParam(string tag, string value, TextPlayback txtPlayback)
        {
            float floatVal;
            
            switch (tag)
            {
                case PauseTextTag:
                    floatVal = float.Parse(value);
                    StartCoroutine(txtPlayback.PausePlayback(floatVal));
                    break;
                case TimeToChangeTag:
                    floatVal = float.Parse(value);
                    floatVal = floatVal < 0 ? TimeToChangeDef : floatVal;
                    timeToChange = floatVal;
                    break;
                case JitterTag:
                    floatVal = float.Parse(value);
                    floatVal = floatVal < 0 ? JitterDef : floatVal;
                    StopAllCoroutines();
                    StartCoroutine(SmoothChangeJitter(floatVal));
                    break;
                case JitterBeatTag:
                    floatVal = float.Parse(value);
                    floatVal = floatVal < 0 ? JitterBeatDef : floatVal;
                    StartCoroutine(SmoothChangeJitterBeat(floatVal));
                    break;
                case JitterRestTag:
                    floatVal = float.Parse(value);
                    floatVal = floatVal < 0 ? JitterRestDef : floatVal;
                    StartCoroutine(SmoothChangeJitterRest(floatVal));
                    break;
                case JitterSyncTag:
                    var isSync = value == SyncOn;
                    _audioJitter.enabled = isSync;
                    break;
                case NoiseTag:
                    floatVal = float.Parse(value);
                    floatVal = floatVal < 0 ? NoiseDef : floatVal;
                    StartCoroutine(SmoothChangeNoiseIntensity(floatVal));
                    break;
                case HeartRateTag:
                    var heartRate = int.Parse(value);
                    heartRate = heartRate < 0 ? BpmDef : heartRate;
                    soundManager.SetHeartRate(heartRate);
                    break;
                case MusicTag:
                    soundManager.SetAndPlayMusic(value);
                    break;
                default:
                    throw new ArgumentException($"There is no method to parse tag: {tag}");
            }
        }

        private IEnumerator SmoothChangeJitter(float val)
        {
            float velocity = 0;

            while (Mathf.Abs(_jitter.jitter - val) > 0.01f)
            {
                _jitter.jitter = Mathf.SmoothDamp(_jitter.jitter, val, ref velocity, timeToChange);
                
                yield return null;
            }
        }
        
        private IEnumerator SmoothChangeJitterBeat(float val)
        {
            float velocity = 0;

            while (Mathf.Abs(_audioJitter.glitchBeatScale - val) > 0.01f)
            {
                _audioJitter.glitchBeatScale = Mathf.SmoothDamp(_audioJitter.glitchBeatScale, val, ref velocity, timeToChange);
                
                yield return null;
            }
        }
        
        private IEnumerator SmoothChangeJitterRest(float val)
        {
            float velocity = 0;

            while (Mathf.Abs(_audioJitter.glitchRestScale - val) > 0.01f)
            {
                _audioJitter.glitchRestScale = Mathf.SmoothDamp(_audioJitter.glitchRestScale, val, ref velocity, timeToChange);
                
                yield return null;
            }
        }
        
        private IEnumerator SmoothChangeNoiseIntensity(float val)
        {
            float velocity = 0;

            while (Mathf.Abs(_noise.grainIntensity - val) > 0.01f)
            {
                _noise.grainIntensity = Mathf.SmoothDamp(_noise.grainIntensity, val, ref velocity, timeToChange);
                
                yield return null;
            }
        }

        private void OnDestroy()
        {
            TextPlayback.TagEncountered -= ParseTagText;
        }
    }
}