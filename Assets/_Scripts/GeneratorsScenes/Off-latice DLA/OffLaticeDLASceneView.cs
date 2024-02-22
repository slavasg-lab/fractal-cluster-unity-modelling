using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OffLaticeDLASceneView : GeneratorSceneView
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private TMP_InputField _sizeInput;
    [SerializeField] private TMP_InputField _countInput;
    [SerializeField] private TMP_InputField _radiusInput;
    private OffLaticeDLAGenerator _offLaticeDlaGenerator;

    private void Awake()
    {
        _offLaticeDlaGenerator = GetComponent<OffLaticeDLAGenerator>();
        _generateButton.onClick.AddListener(delegate
        {
            int size, count;
            float radius;
            if (_sizeInput.text != "" && _countInput.text != "" && _radiusInput.text != "")
            {
                size = Convert.ToInt32(_sizeInput.text);
                count = Convert.ToInt32(_countInput.text);
                radius = (float)Convert.ToDouble(_radiusInput.text);
                ControlsHelper.Instance.SetGeneratedState(false);
                ControlsHelper.Instance.PauseControlls();
                ControlsHelper.Instance.SetCameraPosition(new Vector3(size / 2, size / 2, -50f));
                SetStatusText(GlobalConstants.Generating);
                SetParamsText("Parameters: Size = " + size + "; Count = " + count + "; Radius = " + radius + "\nPress M to open menu");
                SetMenuActive(false);
                _offLaticeDlaGenerator.Generate(size, count, radius, delegate
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
