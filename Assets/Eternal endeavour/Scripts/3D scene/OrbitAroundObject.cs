using UnityEngine;

public class OrbitAroundObject : MonoBehaviour
{
    public GameObject ObjectToRotateAround;

    private float _radiusChangeInterval;
    private float _rotationSpeed;
    private Transform _center;
    private readonly Vector3 _axis = Vector3.forward;
    private Vector3 _desiredPosition;
    private float _lastRadiusChangeTime;
    private float _radius;
    private float _radiusSpeed = 0.5f;
    
    void Start()
    {
        _radius = Random.Range(5, 20);
        _radiusChangeInterval = Random.Range(5, 15);
        _rotationSpeed = Random.Range(5, 80);
        _lastRadiusChangeTime = Time.realtimeSinceStartup;
        _center = ObjectToRotateAround.transform;
        transform.position = (transform.position - _center.position).normalized * _radius + _center.position;
    }

    void Update()
    {
        ChangeRadius();
        transform.RotateAround(_center.position, _axis, _rotationSpeed * Time.deltaTime);
        _desiredPosition = (transform.position - _center.position).normalized * _radius + _center.position;
        transform.position = Vector3.MoveTowards(transform.position, _desiredPosition, Time.deltaTime * _radiusSpeed);
    }

    private void ChangeRadius()
    {
        if (Time.realtimeSinceStartup - _lastRadiusChangeTime > _radiusChangeInterval)
        {
            _radius = Random.Range(5, 20);
            _lastRadiusChangeTime = Time.realtimeSinceStartup;
        }
    }
}