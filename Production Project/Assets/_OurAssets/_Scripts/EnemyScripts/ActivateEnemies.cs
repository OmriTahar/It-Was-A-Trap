using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemies : MonoBehaviour
{
    public List<BaseEnemyAI> EnemiesToActivate = new List<BaseEnemyAI>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("activating enemies!");

            foreach (var enemy in EnemiesToActivate)
            {
                if (enemy != null)
                    enemy.IsEnemyActivated = true;
            }

            Destroy(gameObject);
        }
    }
}
