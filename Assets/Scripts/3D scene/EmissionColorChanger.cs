using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionColorChanger : MonoBehaviour
{
    public Color Color;
    private Material _material;
	// Use this for initialization
	void Start ()
	{
	   _material = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
	    _material.SetColor("_EmissionColor", Color);
    }
}
