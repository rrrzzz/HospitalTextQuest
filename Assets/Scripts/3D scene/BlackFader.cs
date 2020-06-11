using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackFader : MonoBehaviour
{
    [Range(0, 5)]
    public float fadeTime = 1.8f;

    private Image _img;

    private Image Img
    {
        get
        {
            if (_img == null) _img = GetComponent<Image>();
            return _img;
        }
        set => _img = value;
    }

    // Use this for initialization
	void Start ()
	{
        Img = GetComponent<Image>();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Break))
        {
            SetAlphaOne();
        }
    }

    public IEnumerator FadeToBlack()
    {
        Img.CrossFadeAlpha(1, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
    }

    public IEnumerator FadeFromBlack()
    {
        Img.CrossFadeAlpha(0, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
    }

    public void SetAlphaOne()
    {
        var color = Img.color;
        color.a = 1;
        Img.color = color;
    }

    public void SetAlphaZero()
    {
        var color = Img.color;
        color.a = 0;
        Img.color = color;
    }
}