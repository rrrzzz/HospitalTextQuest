using System;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class VideoPlayback : MonoBehaviour
{
    [Range(0.5f, 10)]
    public float ClipPlaybackTime;

    [Range(0, 5)]
    public float ClipPieceTime;

    [Range(0, 53)]
    public float TimeStepRangeStart;

    [Range(0, 53)]
    public float TimeStepRangeEnd;

    private float _playbackStartTime;
    private float _intervalStartTime;
    private VideoPlayer _videoPlayer;
    private bool _shouldStart;

    public static event EventHandler VideoPlaybackFinished;
    public static event EventHandler VideoPlaybackStarted;

    // Use this for initialization
    void Start ()
    {
	    _videoPlayer = GetComponent<VideoPlayer>();
        ScreenManager.FadeToBlackComplete += PlayVideo;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (_shouldStart && _videoPlayer.isPrepared && !_videoPlayer.isPlaying)
	    {
	        _videoPlayer.time = (_videoPlayer.time + GetRandomTimeStep()) % _videoPlayer.clip.length;
            _videoPlayer.Play();
	        OnVideoPlaybackStarted();
	        _playbackStartTime = _intervalStartTime = Time.realtimeSinceStartup;
	    }

	    if (_videoPlayer.isPlaying && CheckIntervalTimeExpired())
	    {
	        _videoPlayer.time = (_videoPlayer.time + GetRandomTimeStep()) % _videoPlayer.clip.length;
	        _intervalStartTime = Time.realtimeSinceStartup;
	    }

	    if (_videoPlayer.isPlaying && CheckPlaybackTimeExpired())
	    {
            _shouldStart = false;
	        _videoPlayer.Stop();
            OnVideoPlaybackFinished();
        }
	}

	private void OnDestroy()
	{
		ScreenManager.FadeToBlackComplete -= PlayVideo;
	}

	public void PreparePlayer()
    {
        _videoPlayer.Prepare();
    }

	private void PlayVideo(object sender, EventArgs e)
    {
        _videoPlayer.Prepare();
        _shouldStart = true;
    }

    private void OnVideoPlaybackStarted()
    {
	    var handler = VideoPlaybackStarted;
	    handler?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnVideoPlaybackFinished()
    {
        var handler = VideoPlaybackFinished;
        handler?.Invoke(this, EventArgs.Empty);
    }

    bool CheckPlaybackTimeExpired() => Time.realtimeSinceStartup - _playbackStartTime > ClipPlaybackTime;

    bool CheckIntervalTimeExpired() => Time.realtimeSinceStartup - _intervalStartTime > ClipPieceTime;

    double GetRandomTimeStep() => Random.Range(TimeStepRangeStart, TimeStepRangeEnd);
}