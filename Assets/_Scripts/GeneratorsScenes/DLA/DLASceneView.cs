using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(DLAGenerator))]
public class DLASceneView : GeneratorSceneView
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private TMP_InputField _sizeInput;
    [SerializeField] private TMP_InputField _countInput;
    [SerializeField] private Button _isFastGeneraingCheckBox;
    [SerializeField] private GameObject _activeCheckBoxGO;
    private DLAGenerator _dlaGenerator;
    private bool _isFastGenerating;

    private void Awake()
    {
        _dlaGenerator = GetComponent<DLAGenerator>();
        _generateButton.onClick.AddListener(delegate
        {
            int size, count;
            if (_sizeInput.text != "" && _countInput.text != "")
            {
                size = Convert.ToInt32(_sizeInput.text);
                count = Convert.ToInt32(_countInput.text);
                ControlsHelper.Instance.SetGeneratedState(false);
                ControlsHelper.Instance.PauseControlls();
                ControlsHelper.Instance.SetCameraPosition(new Vector3(size / 2f, size / 2f, -50f));
                SetStatusText(GlobalConstants.Generating);
                SetParamsText("Parameters: Size = " + size + "; Count = " + count + "\nPress M to open menu");
                SetMenuActive(false);
                _dlaGenerator.Generate(size, count, delegate
                {
                    SetStatusText(GlobalConstants.Generated);
                    ControlsHelper.Instance.SetGeneratedState(true);
                    ControlsHelper.Instance.UnPauseControlls();
                });    
            }
            else
            {
                SetStatusText(GlobalConstants.InputError);
            }
        });
        _isFastGeneraingCheckBox.onClick.AddListener(delegate
        {
            _isFastGenerating = !_isFastGenerating;
            _activeCheckBoxGO.SetActive(_isFastGenerating);
            _dlaGenerator.SetIsFastGenerating(_isFastGenerating);
        });
    }
}
