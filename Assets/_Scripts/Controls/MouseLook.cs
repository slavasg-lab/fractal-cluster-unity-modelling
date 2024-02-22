using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 100f;
    private Transform _transform;
    private float _xRotation = 0f;
    private bool _pause = true;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_pause)
            return;
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        var localRotation = _transform.localRotation;
        _transform.localRotation = Quaternion.Euler(_xRotation, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
        _transform.Rotate(Vector3.up * mouseX, Space.World);
    }

    public void SetPause(bool pause)
    {
        _pause = pause;
    }
}
