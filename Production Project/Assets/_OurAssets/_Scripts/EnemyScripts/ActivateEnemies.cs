using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateEnemies : MonoBehaviour
{
    public List<EnemyAI> EnemiesToActivate = new List<EnemyAI>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (var enemy in EnemiesToActivate)
            {
                print("activating enemies!");
                enemy.IsEnemyActivated = true;
            }
        }
    }
}
