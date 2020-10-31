using System.Linq;
using UnityEngine;

namespace _3D_scene
{
    public class LightsController : MonoBehaviour
    {
        //public GameObject[] LightObjects;
        public GameObject LampsParent;

        [Range(0, 5)]
        public float TimeDeltaSecs;
        [Range(0, 10)]
        public float StartDelay;

        private float _startingTime;
        private int _currentIndex;
        private bool _isStarted;

        // Use this for initialization
        void Start ()
        {
            _startingTime = Time.realtimeSinceStartup;
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (Time.realtimeSinceStartup - _startingTime < StartDelay && !_isStarted) return;
            if (Time.realtimeSinceStartup - _startingTime > StartDelay && !_isStarted)
            {
                _isStarted = true;
                _startingTime = Time.realtimeSinceStartup;
            }

            if (_currentIndex == LampsParent.transform.childCount) gameObject.SetActive(false);

            if (Time.realtimeSinceStartup - _startingTime > TimeDeltaSecs)
            {
                _startingTime = Time.realtimeSinceStartup;
                DisableCurrentLightAndChildren(LampsParent.transform.GetChild(_currentIndex++).gameObject);
            }
        }

        private void DisableCurrentLightAndChildren(GameObject parent)
        {
            var lampTransforms = parent.GetComponentsInChildren<Transform>(false).Where(x => x.gameObject.CompareTag("Lamp"));

            foreach (var lampTransform in lampTransforms)
            {
                DisableCurrentLight(lampTransform.gameObject);
            }
        }

        private void DisableCurrentLight(GameObject lampObject)
        {
            lampObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            lampObject.GetComponentInChildren<Light>().gameObject.SetActive(false);
            lampObject.GetComponent<AudioSource>().Play();
        }
    }
}