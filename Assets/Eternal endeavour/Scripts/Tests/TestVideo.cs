using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class TestVideo : MonoBehaviour
{
    [Range(0.5f, 10)]
    public float ClipPlaybackTime;

    [Range(0,5)]
    public float ClipPieceTime;

    [Range(0, 53)]
    public float TimeStepRangeStart;

    [Range(0,53)]
    public float TimeStepRangeEnd;
        
    private VideoPlayer _videoPlayer;
    private float _playbackStartTime;
    private float _intervalStartTime;

    // Use this for initialization
    void Start ()
	{
	    _videoPlayer = GetComponent<VideoPlayer>();
	    Prepare();
	}
	
	// Update is called once per frame
	void Update () {
	    if (_videoPlayer.isPrepared && !_videoPlayer.isPlaying)
        {
            _videoPlayer.time = (_videoPlayer.time + GetRandomTimeStep()) % _videoPlayer.clip.length;
            _videoPlayer.Play();
            _playbackStartTime = _intervalStartTime = Time.realtimeSinceStartup;
        }

        if (_videoPlayer.isPlaying && CheckIntervalTimeExpired())
        {
            _videoPlayer.time = (_videoPlayer.time + GetRandomTimeStep()) % _videoPlayer.clip.length;
            _intervalStartTime = Time.realtimeSinceStartup;
        }

        if (_videoPlayer.isPlaying && CheckPlaybackTimeExpired())
        {
            _videoPlayer.Stop();
        }
    }

    public void Prepare()
    {
        _videoPlayer.Prepare();
    }

    bool CheckPlaybackTimeExpired() => Time.realtimeSinceStartup - _playbackStartTime > ClipPlaybackTime;

    bool CheckIntervalTimeExpired() => Time.realtimeSinceStartup - _intervalStartTime > ClipPieceTime;

    double GetRandomTimeStep() => Random.Range(TimeStepRangeStart, TimeStepRangeEnd);
}