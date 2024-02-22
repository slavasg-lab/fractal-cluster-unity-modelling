using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneView : MonoBehaviour
{
    [SerializeField] private Button _DLAButton;
    [SerializeField] private Button _OffLaticeDLAButton;
    [SerializeField] private Button _DCLAButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button openHintButton;
    [SerializeField] private Button closeHintButton;
    [SerializeField] private GameObject hintGameObject;

    private void Awake()
    {
        _DLAButton.onClick.RemoveAllListeners();
        _OffLaticeDLAButton.onClick.RemoveAllListeners();
        _DCLAButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        openHintButton.onClick.RemoveAllListeners();
        closeHintButton.onClick.RemoveAllListeners();
        _DLAButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("DLAScene"); 
            ControlsHelper.Instance.SetGeneratedState(false);
        });
        _OffLaticeDLAButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("OffLaticeDLAScene"); 
            ControlsHelper.Instance.SetGeneratedState(false);
        });
        _DCLAButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("DCLAScene"); 
            ControlsHelper.Instance.SetGeneratedState(false);
        });
        quitButton.onClick.AddListener(Application.Quit);
        openHintButton.onClick.AddListener(delegate
        {
            hintGameObject.SetActive(true);
        });
        closeHintButton.onClick.AddListener(delegate
        {
            hintGameObject.SetActive(false);
        });
    }
}
