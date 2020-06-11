using System;
using System.Collections;
using UnityEngine;

public class WakeUpController : MonoBehaviour
{
    public float DelayBetweenWords = 2.5f;
    public float HummingListeningLength;
    public AudioClip[] WakeUpClips;
    public GameObject ViewFader;
    public GameObject EndingCam;
    public GameObject FpsController;
    public GameObject Medbox;
    public GameObject Atm;
    public GameObject EndingLightsParent;
    public bool IsDebugModeOn;

    private BlackFader _faderScript;
    private AudioSource _fpsAudioSource;

	// Use this for initialization
	void Start ()
	{
	    _faderScript = ViewFader.GetComponent<BlackFader>();
	    _fpsAudioSource = FpsController.GetComponent<AudioSource>();
	    TrueEnding.LightsAnimationStartedEvent += StartWakingUp;
	}

    void Update()
    {
        if (IsDebugModeOn)
        {
            _faderScript.SetAlphaOne();
            StartCoroutine(_faderScript.FadeFromBlack());
            Array.ForEach(EndingCam.GetComponentsInChildren<Animator>(), x => x.speed = 10);
            EndingCam.SetActive(true);
            EnableCameraAndEyelidsAnimations();
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TrueEnding.LightsAnimationStartedEvent -= StartWakingUp;
    }

    void StartWakingUp(object sender, float animationTime)
    {
        StartCoroutine(StartWakeUpSequence(animationTime));
    }

    IEnumerator StartWakeUpSequence(float animationTime)
    {
        yield return StartCoroutine(PlayWakeUpVoice(animationTime));
        yield return StartCoroutine(_faderScript.FadeToBlack());
        EndingLightsParent.SetActive(false);
        ChangeCameraToFinal();
        _faderScript.SetAlphaZero();
        Medbox.SetActive(false);
        yield return StartCoroutine(EnableAtmListenHumming());
        EnableCameraAndEyelidsAnimations();
    }

    IEnumerator PlayWakeUpVoice(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);

        foreach (var audioClip in WakeUpClips)
        {
            _fpsAudioSource.clip = audioClip;
            _fpsAudioSource.Play();
            yield return new WaitForSeconds(DelayBetweenWords);
        }
    }

    IEnumerator EnableAtmListenHumming()
    {
        Atm.SetActive(true);
        yield return new WaitForSeconds(HummingListeningLength);
    }

    void EnableCameraAndEyelidsAnimations() => Array.ForEach(EndingCam.GetComponentsInChildren<Animator>(), x => x.enabled = true);

    void ChangeCameraToFinal()
    {
        EndingCam.SetActive(true);
        FpsController.SetActive(false);
    }
}