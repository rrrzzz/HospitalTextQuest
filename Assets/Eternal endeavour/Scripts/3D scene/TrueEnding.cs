using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

public class TrueEnding : MonoBehaviour
{
    public const string WallEndingTag = "WallEnding";
    public const string FloatEndingTag = "FloatEnding";
    
    private const string WallEndingTargetTag = "WallEndingTarget";
    private const string DoorEndingTargetTag = "DoorEndingTarget";
    private const string FogTriggerTag = "FogTrigger";
    private const float RotationToEndingTargetSpeed = 2;

    public GameObject BgMusic;
    public float DelayBeforeEnding;
    public GameObject LightsParent;
    public GameObject FpsCharacter;
    public AudioClip HighPitchRinging;
    public RuntimeAnimatorController DoorEndingController;
    public RuntimeAnimatorController WallEndingController;
    public RuntimeAnimatorController DoorEndingRotationController;
    public RuntimeAnimatorController WallEndingRotationController;
    public GameObject[] ObjectsWithSkyboxAndEmissionAnimators;

    private bool _isEndingInitiated;
    private GameObject _endingTarget;
    private FirstPersonController _fpsControllerScript;
    private bool _shouldStartCameraMovement;
    private Quaternion _desiredRotation;
    private Quaternion _rotationToCheck;
    private Animator _parentAnimator;
    private Animator _myAnimator;
    private Vector3 _offset;
    private GameObject _endingLight;
    
    public static event EventHandler<float> LightsAnimationStartedEvent;
    public static event EventHandler HallEndingReachedEvent;
    public static event EventHandler<FirstPersonController> HallStartedEvent;

    

    // Use this for initialization
    void Start ()
	{
        _parentAnimator = transform.parent.GetComponent<Animator>();
	    _myAnimator = GetComponent<Animator>();
	    _fpsControllerScript = GetComponent<FirstPersonController>();
        OnHallStartedEvent();
    }
	
	// Update is called once per frame
	void Update ()
	{
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
        #endif
        
        //rewrite to use successive coroutines
	    if (_shouldStartCameraMovement)
	    {
            transform.rotation = Quaternion.Slerp(transform.rotation,
	            _desiredRotation, RotationToEndingTargetSpeed * Time.deltaTime);
        }

        if (_shouldStartCameraMovement && Quaternion.Angle(transform.rotation, _rotationToCheck) < 1)
        {
            _shouldStartCameraMovement = false;
            _offset = transform.rotation.eulerAngles;
            _myAnimator.enabled = true;
            _parentAnimator.enabled = true;
        }
    }

    void LateUpdate()
    {
        if (_myAnimator.enabled)
        {
            transform.rotation *= Quaternion.Euler(_offset);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isEndingInitiated) return;

        if (other.CompareTag(FogTriggerTag)) DisableFog();

        if (!other.CompareTag(WallEndingTag) && !other.CompareTag(FloatEndingTag)) return;

        if (other.CompareTag(WallEndingTag))
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            other.gameObject.transform.GetComponentInChildren<Light>().enabled = true;
            _endingLight = other.gameObject;
            _endingTarget = GameObject.FindGameObjectWithTag(WallEndingTargetTag);
            _parentAnimator.runtimeAnimatorController = WallEndingController;
            _myAnimator.runtimeAnimatorController = WallEndingRotationController;
        }
        else
        {
            _endingTarget = GameObject.FindGameObjectWithTag(DoorEndingTargetTag);
            _parentAnimator.runtimeAnimatorController = DoorEndingController;
            _myAnimator.runtimeAnimatorController = DoorEndingRotationController;
        }

        _isEndingInitiated = true;
        StartCoroutine(PlayEndingAnimationAndSound());
    }

    private IEnumerator PlayEndingAnimationAndSound()
    {
        BgMusic.SetActive(false);
        _fpsControllerScript.StopFovKick();
        yield return new WaitForSeconds(DelayBeforeEnding);

        SetEndingRotation();
        _fpsControllerScript.StopInput();
        OnEndingReachedEvent();
        _shouldStartCameraMovement = true;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
    }

    private void SetEndingRotation()
    {
        _desiredRotation = Quaternion.LookRotation(_endingTarget.transform.position - transform.position);

        if (_endingTarget.CompareTag(WallEndingTargetTag))
        {
            _rotationToCheck = _desiredRotation;
            return;
        }

        _rotationToCheck = new Quaternion(_desiredRotation.x * -1, _desiredRotation.y * -1, _desiredRotation.z * -1, _desiredRotation.w * -1);
    }

    private void PlayRingingSound()
    {
        DisableFog();
        var audioSource = GetComponent<AudioSource>();

        audioSource.clip = HighPitchRinging;
        audioSource.Play();
    }

    private void TurnOnFinalLightsAndEmission()
    {
        LightsParent.SetActive(true);

        foreach (var obj in ObjectsWithSkyboxAndEmissionAnimators)
        {
            Array.ForEach(obj.GetComponentsInChildren<Animator>(), x => x.enabled = true);
        }

        var animationLength = LightsParent.GetComponentInChildren<Animator>().runtimeAnimatorController
            .animationClips[0].length;

        OnLightsAnimationStarted(animationLength);

        if (_endingLight != null)
        {
            StartCoroutine(DisableLight());
        }
    }

    private IEnumerator DisableLight()
    {
        yield return new WaitForSeconds(3);
        _endingLight.SetActive(false);
    }

    private void OnLightsAnimationStarted(float animationLength) => LightsAnimationStartedEvent?.Invoke(this, animationLength);

    private void DisableFog() => FpsCharacter.GetComponent<GlobalFog>().enabled = false;

    private void OnEndingReachedEvent()
    {
        HallEndingReachedEvent?.Invoke(null, EventArgs.Empty);
    }

    private void OnHallStartedEvent()
    {
        HallStartedEvent?.Invoke(null, _fpsControllerScript);
    }
}