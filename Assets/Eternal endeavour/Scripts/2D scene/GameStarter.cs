using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public GameObject Managers;
    public GameObject TextFrame;
    public RawImage Black;
    public float PauseBeforeFading = 1;
    public float FadingTime = 1.8f;

    // Use this for initialization
    void Start () {
        Black.color = Color.black;
        TextFrame.GetComponent<Image>().canvasRenderer.SetAlpha(ScreenManager.TextFrameDefaultAlpha);
        StartCoroutine(StartFading());
    }
	
	// Update is called once per frame
	void Update () {
	    if (Black.canvasRenderer.GetAlpha() != 0) return;
        Managers.SetActive(true);
	    gameObject.SetActive(false);
	}

    IEnumerator StartFading()
    {
        yield return new WaitForSeconds(PauseBeforeFading);
        Black.CrossFadeAlpha(0, FadingTime, false);
    }
}
