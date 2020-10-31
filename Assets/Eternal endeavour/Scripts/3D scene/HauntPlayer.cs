using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntPlayer : MonoBehaviour
{
    public Transform player;
    public float playerReachDistance;
    public float movementSpeed;
    public float rotationSpeed;
    public float playerPosOffsetY = 0.5f;

    private bool _isMoving;
    private bool _isRotating;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _isMoving = !_isMoving;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _isRotating = !_isRotating;
        }

        if (_isMoving)
        {
            var pos = player.position;
            pos.y = transform.position.y;
            var distToPlayer = (transform.position - pos).magnitude;
            if (distToPlayer < playerReachDistance)
            {
                Debug.Log("Grabbed");
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, movementSpeed * Time.deltaTime);
            }
        }

        if (_isRotating)
        {
            var pos = player.position;
            pos.y += playerPosOffsetY;
            var dirToPlayer = (pos - transform.position).normalized;
            var rotToPlayer = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, rotationSpeed * Time.deltaTime);
        }
    }
}
