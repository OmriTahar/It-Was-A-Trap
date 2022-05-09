using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Transform PlayerSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SaveManager.Instance.SaveGame(PlayerSpawnPoint);
            Destroy(gameObject);
        }
    }
}