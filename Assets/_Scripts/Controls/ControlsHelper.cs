using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KeyboardMovement))]
[RequireComponent(typeof(MouseLook))]
public class ControlsHelper : MonoBehaviour
{
    private KeyboardMovement _keyboardMovement;
    private MouseLook _mouseLook;
    public static ControlsHelper Instance;
    private bool _isInstanceNotNull;
    private bool _isGenerated;
    private Transform _transform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        _keyboardMovement = GetComponent<KeyboardMovement>();
        _mouseLook = GetComponent<MouseLook>();
        _transform = transform;
        PauseControlls();
    }

    public void PauseControlls()
    {
        Cursor.lockState = CursorLockMode.None;
        _keyboardMovement.SetPause(true);
        _mouseLook.SetPause(true);
    }
    
    public void UnPauseControlls()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _keyboardMovement.SetPause(false);
        _mouseLook.SetPause(false);
    }

    public void SetGeneratedState(bool isGenerated)
    {
        _isGenerated = isGenerated;
    }

    public bool GetGeneratedState()
    {
        return _isGenerated;
    }
    private void Update()
    {
        _isInstanceNotNull = GeneratorSceneView.Instance != null;
        if (_isGenerated && _isInstanceNotNull && Input.GetKeyDown(KeyCode.M))
        {
            if (GeneratorSceneView.Instance.GetMenuActive())
            {
                GeneratorSceneView.Instance.SetMenuActive(false);
                UnPauseControlls();
            }
            else
            {
                GeneratorSceneView.Instance.SetMenuActive(true);
                PauseControlls();
            }
        }
    }

    public void SetCameraPosition(Vector3 position)
    {
        _transform.position = position;
        _transform.rotation = Quaternion.identity;
    }
}
