using UnityEngine;

public class SkyboxColorChanger : MonoBehaviour
{
    public Color BgColor;
    private Camera _cam;
	// Use this for initialization
	void Start ()
	{
	    _cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	    _cam.backgroundColor = BgColor;
    }
}
