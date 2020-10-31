using UnityEngine;

public class BallController : MonoBehaviour
{
    public float Speed;
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var t = transform.position;
        t.x += Input.GetAxis("Horizontal") * Time.deltaTime * Speed;
        t.z += Input.GetAxis("Vertical") * Time.deltaTime * Speed;
        _rb.MovePosition(t);   
    }
}
