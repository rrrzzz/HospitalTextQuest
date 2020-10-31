using System;
using System.Collections;
using UnityEngine;

namespace _2D_scene
{
    public class SoundManager : MonoBehaviour
    {
        private const string AudioRoot = @"Sounds/";
        private const string MriPath = @"mriFinal";
        private const string NeedlePath = @"needle";
        private const int DefBpm = 70;
    
        private AudioSource _audioSrc;
    
        private AudioClip _mriClip;
        private AudioClip _needle;
        private HeartBeater _heartBeater;
    

        // Use this for initialization
        void Start ()
        {
            _audioSrc = GetComponent<AudioSource>();
            _audioSrc.loop = false;
            LoadAudioResources();
            _heartBeater = GetComponent<HeartBeater>();
            SetHeartRate();
            _heartBeater.StartBeating();

            VideoPlayback.VideoPlaybackStarted += PlayMriSound;
            VideoPlayback.VideoPlaybackFinished += ResumeHeartBeat;
        }

        private void LoadAudioResources()
        {
            _mriClip = LoadClip(MriPath);
            _needle = LoadClip(NeedlePath);
        }

        AudioClip LoadClip(string path) => Resources.Load<AudioClip>(AudioRoot + path);

        private void OnDestroy()
        {
            VideoPlayback.VideoPlaybackStarted -= PlayMriSound;
            VideoPlayback.VideoPlaybackFinished -= ResumeHeartBeat;
        }

        void PlayMriSound(object sender, EventArgs e) => PlayClip(_mriClip);
    

        void ResumeHeartBeat(object sender, EventArgs e)
        {
            StartCoroutine(StartBeatingAfterPause());
        }

        IEnumerator StartBeatingAfterPause()
        {
            yield return new WaitForSeconds(0.5f);
            _heartBeater.StartBeating();
        }

        void PlayClip(AudioClip clip)
        {
            _audioSrc.clip = clip;
            _audioSrc.Play();
        }

        public void SetHeartRate(int rate = DefBpm) => _heartBeater.ChangeBpm(rate);

        public void SetAndPlayMusic(string title)
        {
            switch (title)
            {
                case "heart":
                    _heartBeater.StartBeating();
                    break;
                case "needle":
                    _heartBeater.StopBeating();
                    PlayClip(_needle);
                    break;
                case "off":
                    _audioSrc.Stop();
                    break;
            }
        }
    }
}