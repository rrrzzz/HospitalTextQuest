using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    [Range(0, 5)]
    public float FadingTime = 1.8f;

    public Image BackgroundImage;
    public RawImage Black;
    public GameObject TextFrame;
    public GameObject TextMesh;
    public static event EventHandler FadeToBlackComplete;

    private TextPlayback _textScript;
    private bool _isFadeToBlackComplete;
    private bool _isFadingToTransparent;
    private Image _frameImage;
    public const float TextFrameDefaultAlpha = 0.8f;
    private const float TextFrameAlphaFadeTime = 0.5f;
    
    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _textScript = TextMesh.GetComponent<TextPlayback>();
        _textScript.IsExecuting = true;
        _frameImage = TextFrame.GetComponent<Image>();
        TextPlayback.NewScreenEncountered += BeginScreenTransition;
        VideoPlayback.VideoPlaybackFinished += FinishTransition;
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (Black.canvasRenderer.GetAlpha() == 1 && !_isFadeToBlackComplete)
        {
            ContinueTransition();
            _isFadeToBlackComplete = true;
            OnFadeToBlackComplete();
	    }

	    if (_isFadingToTransparent && Black.canvasRenderer.GetAlpha() == 0)
	    {
	        EnableTextFrameAndPlayback();
	    }
	}

    private void OnDestroy()
    {
        TextPlayback.NewScreenEncountered -= BeginScreenTransition;
        VideoPlayback.VideoPlaybackFinished -= FinishTransition;
    }

    private void OnFadeToBlackComplete()
    {
        var handler = FadeToBlackComplete;
        handler?.Invoke(this, EventArgs.Empty);
    }

    private void BeginScreenTransition(object sender, EventArgs e)
    {
        _textScript.IsExecuting = false;
        Black.CrossFadeAlpha(1, FadingTime, false);
    }

    private void ContinueTransition()
    {
        BackgroundImage.canvasRenderer.SetAlpha(0);
        Black.gameObject.SetActive(false);
        TextFrame.SetActive(false);
        _frameImage.canvasRenderer.SetAlpha(0);
    }

    private void CheckEndingOrNewSceneNeeded()
    {
        if (_textScript.CurrentState.name == TextPlayback.GameCloseStateName)
        {
            Application.Quit();
        }

        if (_textScript.CurrentState.name == TextPlayback.TrueEndingStateName)
        {
            ChoiceRecorder.ShouldReplayChoices = true;
            ChoiceRecorder.ChoiceListIndex = 0;
            SceneManager.LoadScene(1);
        }
    }

    private void FinishTransition(object sender, EventArgs e)
    {
        Black.gameObject.SetActive(true);
        _textScript.Initialize();
        CheckEndingOrNewSceneNeeded();
        _isFadingToTransparent = true;
        BackgroundImage.sprite = _textScript.CurrentState.BackgroundSprite;
        BackgroundImage.canvasRenderer.SetAlpha(1);
        Black.CrossFadeAlpha(0, FadingTime, false);
    }

    private void EnableTextFrameAndPlayback()
    {
        _isFadeToBlackComplete = false;
        _isFadingToTransparent = false;
        StartCoroutine(StartPlayback());
    }

    private IEnumerator StartPlayback()
    {
        TextFrame.SetActive(true);
        _frameImage.CrossFadeAlpha(TextFrameDefaultAlpha, TextFrameAlphaFadeTime, true);
        yield return new WaitForSeconds(1);
        _textScript.IsExecuting = true;
    }
}