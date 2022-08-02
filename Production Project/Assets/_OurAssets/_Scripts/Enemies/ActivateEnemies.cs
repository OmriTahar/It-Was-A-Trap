using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ActivateEnemies : MonoBehaviour
{

    public List<BaseEnemyAI> EnemiesToActivate = new List<BaseEnemyAI>();

    [SerializeField] TextMeshProUGUI _bunniesToKillCounter;
    [SerializeField] Animator _bunnyPanelAnimator;

    private bool _isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_isTriggered)
        {
            _isTriggered = true;

            int sum = int.Parse(_bunniesToKillCounter.text) + int.Parse(EnemiesToActivate.Count.ToString());
            _bunniesToKillCounter.text = sum.ToString();

            print("activating enemies!");
            _bunnyPanelAnimator.Play("Highlight", 0);

            foreach (var enemy in EnemiesToActivate)
            {
                if (enemy != null)
                    enemy.IsEnemyActivated = true;
            }

            Invoke("DestorySelf", 6);
        }
    }

    void DestorySelf()
    {
        Destroy(gameObject);
    }


}
