using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] GameObject PlayerGO;
    public SaveFile CurrentSave;

    private PlayerData _playerData;
    private string _save1, _save2, _save3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        _save1 = Application.dataPath + "/Resources/Save.txt";
        //_save2 = Application.dataPath + "/Resources/Save2.txt";
        //_save3 = Application.dataPath + "/Resources/Save3.txt";

        _playerData = PlayerGO.GetComponent<PlayerData>();
    }

    [ContextMenu("Save")]
    public void SaveGame(Transform newSpawnPoint)
    {
        CurrentSave = new SaveFile(newSpawnPoint, _playerData);

        string savedString = JsonUtility.ToJson(CurrentSave);

        if (!File.Exists(_save1))
        {
            StreamWriter sW = File.CreateText(_save1);
            sW.Write(savedString);
            sW.Close();
        }
        else
        {
            File.WriteAllText(_save1, savedString);
        }
    }

    [ContextMenu("Load")]
    public void LoadGame()
    {
        string savedData = null;

        if (File.Exists(_save1))
            savedData = File.ReadAllText(_save1);

        JsonUtility.FromJsonOverwrite(savedData, CurrentSave);

        PlayerGO.transform.position = CurrentSave.position;
        PlayerGO.transform.rotation = CurrentSave.rotation;
        _playerData._unitHP = CurrentSave.health;
        _playerData.bunnyCount = CurrentSave.bunnyCount;
        _playerData.currentWeapon = CurrentSave.lastUsedWeapon;
    }

}