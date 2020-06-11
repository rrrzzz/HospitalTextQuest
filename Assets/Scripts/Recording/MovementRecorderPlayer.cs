using System;
using System.Collections.Generic;
using Recording;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MovementRecorderPlayer : MonoBehaviour
{
    public bool isStartRecordingHall;
    public bool isStartRecordingBed;
    public bool isStartPlaybackHall;
    public bool isStartPlaybackBed;
    public bool isFirstTime = true;
    
    private static List<PointInTime> _hallMovementsCam = new List<PointInTime>();
    private static List<PointInTime> _hallMovementsFps = new List<PointInTime>();
    private static readonly List<Quaternion> BedLookRotations = new List<Quaternion>();
    private int _currentFrame;
    private Transform _mainCam;
    private Transform _FPSControllerTransform;
    private static MovementRecorderPlayer _instance;


    private void Awake()
    {
        if (_instance == null)
        {
            Debug.Log("MovementRecorderPlayer awakes");
            _instance = this;
            DontDestroyOnLoad(gameObject);
            TrueEnding.HallEndingReachedEvent += OnHallEndingReached;
            TrueEnding.HallStartedEvent += OnHallStarted;
            EndingCamRingingAndFade.WakingUpFinishedEvent += OnWakingUpFinished;
            EndingCamRingingAndFade.WakingUpCamActivatedEvent += OnWakingCamActivated;
            DevouringDarknessController.DeathEvent += OnDeath;
            FirstPersonController.FpsControllerAwokeEvent += OnFpsControllerAwoke;
        }

        else if (_instance != this)
        {
            Debug.Log("Unnecessary MovementRecorderPlayer destroyed");
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isStartPlaybackHall)
        {
            if (_currentFrame > _hallMovementsCam.Count - 1) return;
            if (_currentFrame > _hallMovementsFps.Count - 1) return;
            var currentMoveCam = _hallMovementsCam[_currentFrame];
            var currentMoveFps = _hallMovementsFps[_currentFrame++];
            _FPSControllerTransform.position = currentMoveFps.Position;
            _FPSControllerTransform.rotation = currentMoveFps.Rotation;
            _mainCam.position = currentMoveCam.Position;
            _mainCam.rotation = currentMoveCam.Rotation;
        }

        if (isStartPlaybackBed)
        {
            if (_currentFrame > BedLookRotations.Count - 1) return;
            _mainCam.rotation = BedLookRotations[_currentFrame++];
        }

        if (!isFirstTime) return;

        if (isStartRecordingHall)
        {
            _hallMovementsCam.Add(new PointInTime(_mainCam.position, _mainCam.rotation));
            _hallMovementsFps.Add(new PointInTime(_FPSControllerTransform.position, _FPSControllerTransform.rotation));
        }

        if (isStartRecordingBed)
        {
            BedLookRotations.Add(_mainCam.rotation);
        }
    }

    void OnFpsControllerAwoke(object sender, GameObject fpsControllerObject)
    {
        _FPSControllerTransform = fpsControllerObject.transform;
        if (!isFirstTime)
        {
            var script = (FirstPersonController) sender;
            script.StopInput();
        }
    }
    
    void OnHallStarted(object sender, FirstPersonController firstPersonController)
    {
        Debug.Log("hall started");
        _mainCam = Camera.main.transform;
        
        if (isFirstTime)
        {
            isStartRecordingHall = true;
            return;
        }
    
        firstPersonController.StopInput();
        _currentFrame = 0;
        isStartPlaybackHall = true;
    }
     
    private void OnDeath(object sender, EventArgs e)
    {
        isStartRecordingHall = false;
        _hallMovementsCam = new List<PointInTime>();
    }

    private void OnHallEndingReached(object sender, EventArgs e)
    {
        if (!isFirstTime)
        {
            isStartPlaybackHall = false;
            return;
        }
        isStartRecordingHall = false;
    }

    void OnWakingCamActivated(object sender, CamClamper camClamper)
    {
        _mainCam = Camera.main.transform;
        if (!isFirstTime)
        {
            camClamper.enabled = false;
            _currentFrame = 0;
            isStartPlaybackBed = true;
            return;
        }
        isStartRecordingBed = true; 
    }

    void OnWakingUpFinished(object sender, EventArgs e)
    {
        if (!isFirstTime)
        {
            isStartPlaybackBed = false;
            return;
        }
        isStartRecordingBed = false;
        isFirstTime = false;
    }
}