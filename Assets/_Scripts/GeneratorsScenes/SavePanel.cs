using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePanel : MonoBehaviour
{
    [SerializeField] private Savable _generator;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TMP_InputField _fileNameInput;
    [SerializeField] private GeneratorType _generatorType;
    private string SAVE_FOLDER;
    
    private void Start()
    {
        _backButton.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
        _saveButton.onClick.AddListener(delegate
        {
            Save(_generator.GetSaveInfo());
        });
        SAVE_FOLDER  = Application.dataPath + "/Saves/" + _generatorType + "/";
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    private void Save(SaveInfo saveInfo)
    {
        string json = JsonUtility.ToJson(saveInfo);
        if (File.Exists(SAVE_FOLDER + _fileNameInput.text + ".txt"))
        {
            _statusText.text = GlobalConstants.ClasterExist;
            return;
        }
        File.WriteAllText(SAVE_FOLDER + _fileNameInput.text + ".txt", json);
        _statusText.text = GlobalConstants.ClasterSaved;
    }

    private void OnEnable()
    {
        _statusText.text = "-";
    }
}

public enum GeneratorType
{
    DLA,
    OffLaticeDLA,
    DCLA
}
