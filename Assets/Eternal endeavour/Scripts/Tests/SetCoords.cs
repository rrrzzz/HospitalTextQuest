using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCoords : MonoBehaviour
{
    public GameObject Object;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Object.transform.rotation;
        transform.position = Object.transform.position;
    }
}
