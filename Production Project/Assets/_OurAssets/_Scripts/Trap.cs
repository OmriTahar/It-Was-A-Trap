using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Attack
{
    
    
    public override void  OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
           other.GetComponent<EnemyAI>()._unitHP -=_damage ;
            if (other.GetComponent<EnemyAI>()._unitHP <= 0)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
                
            }
        }
    }
    private void OnDestroy()
    {
        PlayerData.Instance._trapAmmo++;
    }
}
