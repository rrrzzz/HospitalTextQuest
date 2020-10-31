using System.Collections;
using UnityEngine;

namespace _2D_scene
{
    public class HeartBeater : MonoBehaviour
    {
        private const string HeartPath = "Sounds/Heart/";

        private AudioClip _systole;
        private AudioClip _diastole;
        private AudioSource _src;
        private bool _isBeating;
        public float _beatLength;

        void Start()
        {
            _src = GetComponent<AudioSource>();
            _systole = Resources.Load<AudioClip>(HeartPath + "sysSuperFast");
            _diastole = Resources.Load<AudioClip>(HeartPath + "dysSuperFast");
        }
    
        public void StartBeating()
        {
            _isBeating = true;
            StartCoroutine(PlayHeartbeat());
        }

        public void StopBeating() => _isBeating = false;

        public void ChangeBpm(int bpm) => _beatLength = 60f / bpm;

        IEnumerator PlayHeartbeat()
        {
            while(true)
            {
                if (!_isBeating) break;
                _src.clip = _systole;
                _src.Play(); 
                yield return new WaitForSeconds(_beatLength);
                
                if (!_isBeating) break;
                _src.clip = _diastole;
                _src.Play();
                yield return new WaitForSeconds(_beatLength);
            }
        }
    }
}
