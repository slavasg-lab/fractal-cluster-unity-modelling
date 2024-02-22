using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneratorSceneView : MonoBehaviour
{
    public static GeneratorSceneView Instance;
    [SerializeField] private GameObject _menuPanelGameObject;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _savePanelGameObject;
    [SerializeField] private Button _saveButton;
    [SerializeField] private GameObject _loadPanelGameObject;
    [SerializeField] private Button _loadButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _paramsText;
    [SerializeField] private Button hintButton;
    [SerializeField] private GameObject hintGameObject;
    [SerializeField] private Button closeHintButton;
    [SerializeField] private TextMeshProUGUI dimensionText;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }

        _backButton.onClick.AddListener(delegate { SceneManager.LoadScene("MainScene"); });
        _saveButton.onClick.AddListener(delegate
        {
            if (ControlsHelper.Instance.GetGeneratedState())
            {
                _savePanelGameObject.SetActive(true);
            }
        });
        _loadButton.onClick.AddListener(delegate { _loadPanelGameObject.SetActive(true); });
        hintButton.onClick.AddListener(delegate { SetHintActive(true); });
        closeHintButton.onClick.AddListener(delegate { SetHintActive(false); });
    }

    public void SetMenuActive(bool active)
    {
        _menuPanelGameObject.SetActive(active);
    }

    private void SetHintActive(bool active)
    {
        hintGameObject.SetActive(active);
    }

    public bool GetMenuActive()
    {
        return _menuPanelGameObject.activeSelf;
    }

    public void ClearProgress()
    {
        _progressBar.value = 0f;
    }

    public void SetProgress(float progress)
    {
        _progressBar.value = progress;
    }

    public void SetStatusText(string text)
    {
        _statusText.text = text;
    }

    public void SetParamsText(string text)
    {
        _paramsText.text = text;
    }

    public void SetDimensionText(float dimension)
    {
        dimensionText.text = "D = " + Math.Round(dimension + 1, 2);
    }
}
