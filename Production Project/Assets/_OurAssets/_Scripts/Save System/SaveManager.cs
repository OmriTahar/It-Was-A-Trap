using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveFile CurrentSave;
    string Path;
    [SerializeField] bool AutoLoad = false;

    [SerializeField] GameObject PlayerGO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Path = Application.dataPath + "/Resources/Save.txt";

        Instance = this;

        if (File.Exists(Path) && AutoLoad)
        {
            LoadGame();
        }
    }

    [ContextMenu("Save ME! >:(")]
    public void SaveGame()
    {
        CurrentSave = new SaveFile(PlayerGO);

        string savedString = JsonUtility.ToJson(CurrentSave);

        if (!File.Exists(Path))
        {
            StreamWriter sW = File.CreateText(Path);
            sW.Write(savedString);
            sW.Close();
        }
        else
        {
            File.WriteAllText(Path, savedString);
        }
    }

    [ContextMenu("Load Save.")]
    public void LoadGame()
    {
        string savedData = File.ReadAllText(Path);

        JsonUtility.FromJsonOverwrite(savedData, CurrentSave);

        PlayerGO.transform.position = CurrentSave.position;
        PlayerGO.transform.rotation = CurrentSave.rotation;
    }

}