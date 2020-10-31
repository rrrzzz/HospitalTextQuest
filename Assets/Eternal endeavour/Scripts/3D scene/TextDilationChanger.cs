using TMPro;
using UnityEngine;

public class TextDilationChanger : MonoBehaviour
{
    [SerializeField] private float _dilateUpperBound = 0.55f;

    private TextMeshPro _textMeshPro;
    private Material _textProMaterial;
    private float _velocity;
    private bool _isTargetReached;

    [Range(0,5)]
    public float DilateTargetReachTime;

    // Use this for initialization
    void Start ()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        _textProMaterial = _textMeshPro.fontMaterial;
        _textProMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_isTargetReached && _textProMaterial.GetFloat(ShaderUtilities.ID_FaceDilate) < -0.9f)
	    {
            Destroy(gameObject);
	    }

	    if (_textProMaterial.GetFloat(ShaderUtilities.ID_FaceDilate) > _dilateUpperBound - 0.05)
	    {
	        _dilateUpperBound = -1f;
	        _isTargetReached = true;
	    }

	    var currentValue = Mathf.SmoothDamp(_textProMaterial.GetFloat(ShaderUtilities.ID_FaceDilate), _dilateUpperBound, ref _velocity, DilateTargetReachTime);
	    _textProMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, currentValue);
    }
}