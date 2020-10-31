using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class DevouringDarknessController : MonoBehaviour
{
    [Range(0, 10)] public float DarknessDevourDelay;

    public AudioClip Scream;
    public GameObject BgMusic;
    public GameObject GameOverText;
    public float RotationCompetionTime;
    public float CrouchCompletionTime;
    public GameObject FpsCharacter;

    private FirstPersonController _fpsController;
    private AudioSource _audioSource;
    private float _darknessStartTime;
    private bool _isInDarkness;
    private bool _isScreamStarted;
    private bool _isGameOverTextShown;
    private float _xRotationVelocity;
    private float _yCrouchingVelocity;

    public static event EventHandler DeathEvent;

    // Use this for initialization
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _fpsController = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Fix time slowdown here, door animation is slow too! Reset all params.
        if (_isGameOverTextShown)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ReturnToDefaultTime();
                OnDeathEvent();
                SceneManager.LoadScene(1);
            }
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        else if (_isScreamStarted && !_audioSource.isPlaying)
        {
           GameOverText.SetActive(true);
           _isGameOverTextShown = true;
        }
    }

    private void ReturnToDefaultTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(TrueEnding.WallEndingTag) || other.CompareTag(TrueEnding.FloatEndingTag)) return;
        if (!other.CompareTag("Lamp") || _isScreamStarted || !gameObject.activeInHierarchy) return;

        var isLampLit = other.gameObject.transform.GetChild(0).gameObject.activeSelf;
        if (!_isInDarkness && !isLampLit) _darknessStartTime = Time.realtimeSinceStartup;
        _isInDarkness = !isLampLit;

        if (_isInDarkness && Time.realtimeSinceStartup - _darknessStartTime > DarknessDevourDelay)
        {
            EnactDeath();
        }
    }

    private void EnactDeath()
    {
        BgMusic.SetActive(false);
        _fpsController.enabled = false;
        _audioSource.clip = Scream;
        _audioSource.Play();
        _isScreamStarted = true;
        StartCoroutine(StartDeathAnimation());
    }

    IEnumerator StartDeathAnimation()
    {
        yield return StartCoroutine(Crouch());
        yield return StartCoroutine(Rotate());
    }

    IEnumerator Crouch()
    {
        while (FpsCharacter.transform.position.y > .3f)
        {
            float posY = Mathf.SmoothDamp(FpsCharacter.transform.position.y, .2f, ref _yCrouchingVelocity, CrouchCompletionTime);
            FpsCharacter.transform.position = new Vector3(FpsCharacter.transform.position.x, posY, FpsCharacter.transform.position.z);
            yield return null;
        }
    }

    IEnumerator Rotate()
    {
        var desiredAngle = 45;
        var fpsCharEulerX = FpsCharacter.transform.eulerAngles.x;
        if (fpsCharEulerX > desiredAngle && fpsCharEulerX <= 90) yield return null;
        else
        {
            while (Mathf.Abs(FpsCharacter.transform.eulerAngles.x - desiredAngle) > 1)
            {
                var anglesToRotate = fpsCharEulerX >= 270 ? desiredAngle + 360 - fpsCharEulerX : desiredAngle - fpsCharEulerX;
                FpsCharacter.transform.Rotate(anglesToRotate * Time.deltaTime / RotationCompetionTime, 0, 0);
                yield return null;
            }
        }
    }

    private void OnDeathEvent()
    {
        DeathEvent?.Invoke(null, EventArgs.Empty);
    }
}