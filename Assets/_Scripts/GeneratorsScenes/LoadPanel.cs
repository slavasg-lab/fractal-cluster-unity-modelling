using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

public class LoadPanel : MonoBehaviour
{
    [SerializeField] private Savable _generator;
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _saveRoot;
    [SerializeField] private GameObject _savePrefab;
    [SerializeField] private GeneratorType _generatorType;
    private string SAVE_FOLDER;
    
    private void Awake()
    {
        _backButton.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
        SAVE_FOLDER  = Application.dataPath + "/Saves/" + _generatorType + "/";
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public void OnEnable()
    {
        Utils.DestroyAllChildren(_saveRoot);
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        List<FileInfo> saveFiles = new List<FileInfo>(directoryInfo.GetFiles());
        saveFiles.Sort((fileInfoA, fileInfoB) =>
            fileInfoA.LastWriteTime.CompareTo(fileInfoB.LastWriteTime));
        string saveString;
        foreach (var saveFile in saveFiles)
        {
            try
            {
                saveString = File.ReadAllText(saveFile.FullName);
                SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(saveString);
                SaveView saveView = Instantiate(_savePrefab, _saveRoot).GetComponent<SaveView>();
                saveView.Configure(saveFile.Name, saveInfo, delegate(SaveInfo info)
                {
                    GeneratorSceneView.Instance.ClearProgress();
                    _generator.LoadInfo(info);
                });
            }
            catch
            {
                // ignored
            }
        }
    }
}
