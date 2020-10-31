using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCamRingingAndFade : MonoBehaviour
{

    public static event EventHandler<CamClamper> WakingUpCamActivatedEvent;
    public static event EventHandler WakingUpFinishedEvent;
    
    public GameObject ViewFader;
    public float RingingDuration = 1.5f;
    public GameObject FreelookEndingCamParent;
    public float FreeLookDuration = 10f;

    private GameObject _eyelids;

    void Start()
    {
        _eyelids = transform.GetChild(0).gameObject;
    }

    void StartRingingEnding()
    {
        _eyelids.SetActive(false);
        GetComponent<Camera>().enabled = false;
        FreelookEndingCamParent.SetActive(true);
        OnWakingUpCamActivatedEvent();
        StartCoroutine(PlayRingingAndFadeTo2D());
    }

    IEnumerator PlayRingingAndFadeTo2D()
    {
        yield return new WaitForSeconds(FreeLookDuration);

        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(RingingDuration);
        yield return StartCoroutine(ViewFader.GetComponent<BlackFader>().FadeToBlack());
        OnRingingFinishedEvent();
        //debug fadetoblack, now it's instant change
        //reset global variable for drugged sinus shader
        SceneManager.LoadScene(0);
    }

    private void OnRingingFinishedEvent()
    {
        WakingUpFinishedEvent?.Invoke(null, EventArgs.Empty);
    }

    private void OnWakingUpCamActivatedEvent()
    {
        WakingUpCamActivatedEvent?.Invoke(null, FreelookEndingCamParent.GetComponent<CamClamper>());
    }
}