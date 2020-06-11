using UnityEngine;

public class StartSlowMoAndFadingFromBlack : MonoBehaviour
{
    public GameObject ViewFader;
    public float TimeScale = 0.5f;

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(ViewFader.GetComponent<BlackFader>().FadeFromBlack());
	    Time.timeScale = TimeScale;
	    Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
}