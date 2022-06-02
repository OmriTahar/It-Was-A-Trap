using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public Vector3 position;
    public Quaternion rotation;
    public Weapon lastUsedWeapon;
    public int bunnyCount;
    public float health;

    public SaveFile()
    {

    }

    public SaveFile(GameObject player)
    {
        position = player.transform.position;
        rotation = player.transform.rotation;
    }

    public SaveFile(Transform spawnPoint)
    {
        position = spawnPoint.position;
        rotation = spawnPoint.rotation;
    }

    public SaveFile(Transform spawnPoint, PlayerData playerData)
    {
        position = spawnPoint.position;
        rotation = spawnPoint.rotation;
        health = playerData._unitHP;
        bunnyCount = playerData.bunnyCount;
        lastUsedWeapon = playerData.currentWeapon;
    }
}