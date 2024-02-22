using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DCLASceneView : GeneratorSceneView
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private TMP_InputField _sizeInput;
    [SerializeField] private TMP_InputField _countInput;
    private DCLAGenerator _dclaGenerator;

    private void Awake()
    {
        _dclaGenerator = GetComponent<DCLAGenerator>();
        _generateButton.onClick.AddListener(delegate
        {
            int size, count;
            if (_sizeInput.text != "" && _countInput.text != "")
            {
                size = Convert.ToInt32(_sizeInput.text);
                count = Convert.ToInt32(_countInput.text);
                ControlsHelper.Instance.SetGeneratedState(false);
                ControlsHelper.Instance.PauseControlls();
                ControlsHelper.Instance.SetCameraPosition(new Vector3(size / 2, size / 2, -50f));
                SetStatusText(GlobalConstants.Generating);
                SetParamsText("Parameters: Size = " + size + "; Count = " + count + "\nPress M to open menu");
                SetMenuActive(false);
                _dclaGenerator.Generate(size, count, delegate
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
    }
}
