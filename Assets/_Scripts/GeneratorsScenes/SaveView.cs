using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _saveNameText;
    [SerializeField] private Button _loadButton;
    public void Configure(string name, SaveInfo saveInfo, Action<SaveInfo> onLoadButton)
    {
        _loadButton.onClick.AddListener(delegate
        {
            onLoadButton?.Invoke(saveInfo);
        });
        _saveNameText.text = name;
    }
}
