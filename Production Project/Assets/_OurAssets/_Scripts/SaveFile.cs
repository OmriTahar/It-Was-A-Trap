using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public Vector3 position;
    public Quaternion rotation;

    public SaveFile()
    {

    }

    public SaveFile(GameObject player)
    {
        position = player.transform.position;
        rotation = player.transform.rotation;
    }
}