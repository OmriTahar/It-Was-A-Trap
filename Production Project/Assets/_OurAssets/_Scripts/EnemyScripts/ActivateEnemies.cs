using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemies : MonoBehaviour
{
    public List<BaseEnemyAI> EnemiesToActivate = new List<BaseEnemyAI>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (var enemy in EnemiesToActivate)
            {
                print("activating enemies!");
                enemy.IsEnemyActivated = true;
            }

            Destroy(gameObject);
        }
    }
}
