using UnityEngine;

public class CamClamper : MonoBehaviour
{
    public float MinimumX = 2f;
    public float MaximumX = 90f;
    public float MinimumY = -90f;
    public float MaximumY = 90f;

    private float _camRotX;
    private float _charRotY;

    private Transform _camChild;

    private float _sensitivity = 2f;
    // Start is called before the first frame update
    void Start()
    {
        _camChild = transform.GetChild(0);
    }

    //Takes into account initial rotation of camera and its parent
    void Update()
    {
        var currentRotY = Input.GetAxis("Mouse X") * _sensitivity;
        var currentRotX = Input.GetAxis("Mouse Y") * _sensitivity;

        if (_charRotY + currentRotY <= MinimumY)
        {
            currentRotY = MinimumY - _charRotY;
            _charRotY = MinimumY;
        }
        else if (_charRotY + currentRotY >= MaximumY)
        {
            currentRotY = MaximumY - _charRotY;
            _charRotY = MaximumY;
        }
        else
        {
            _charRotY += currentRotY;
        }

        if (_camRotX + currentRotX <= MinimumX)
        {
            currentRotX = MinimumX - _camRotX;
            _camRotX = MinimumX;
        }
        else if (_camRotX + currentRotX >= MaximumX)
        {
            currentRotX = MaximumX - _camRotX;
            _camRotX = MaximumX;
        }
        else
        {
            _camRotX += currentRotX;
        }

        transform.localRotation *= Quaternion.Euler(0, currentRotY, 0);
        _camChild.localRotation *= Quaternion.Euler(-currentRotX, 0, 0);
    }
}