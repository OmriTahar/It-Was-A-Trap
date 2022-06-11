using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] GameObject PlayerGO;
    public SaveFile CurrentSave;
    public bool AutoLoad = false;
    NewPlayerData playerData;
    string Path;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Path = Application.dataPath + "/Resources/Save.txt";
        playerData = PlayerGO.GetComponent<NewPlayerData>();

        if (File.Exists(Path) && AutoLoad)
        {
            LoadGame();
        }
    }

    [ContextMenu("Save")]
    public void SaveGame(Transform newSpawnPoint)
    {
        CurrentSave = new SaveFile(newSpawnPoint, playerData);

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

    [ContextMenu("Load")]
    public void LoadGame()
    {
        string savedData = File.ReadAllText(Path);

        JsonUtility.FromJsonOverwrite(savedData, CurrentSave);

        PlayerGO.transform.position = CurrentSave.position;
        PlayerGO.transform.rotation = CurrentSave.rotation;
        playerData._unitHP = CurrentSave.health;
        playerData.bunnyCount = CurrentSave.bunnyCount;
        playerData.currentWeapon = CurrentSave.lastUsedWeapon;
    }

}