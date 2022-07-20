using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ActivateEnemies : MonoBehaviour
{

    public List<BaseEnemyAI> EnemiesToActivate = new List<BaseEnemyAI>();

    [SerializeField] TextMeshProUGUI _bunniesToKillCounter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            int sum = int.Parse(_bunniesToKillCounter.text) + int.Parse(EnemiesToActivate.Count.ToString());
            _bunniesToKillCounter.text = sum.ToString();

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
