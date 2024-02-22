using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private Transform _transform;
    private bool _pause = true;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_pause)
            return;
        float x = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
        float y;
        if (Input.GetKey(KeyCode.Space))
        {
            y = _speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            y = -_speed * Time.deltaTime;
        }
        else
        {
            y = 0;
        }
        Vector3 move = _transform.right * x + _transform.forward * z + Vector3.up * y;
        transform.position += move;
    }
    
    public void SetPause(bool pause)
    {
        _pause = pause;
    }
}
