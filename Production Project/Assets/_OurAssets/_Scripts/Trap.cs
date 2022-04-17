using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int trapammo = 2;
    int trapdmg = 50;
    bool isactive = false;
    public GameObject trap;
    public EnemyAI enemy;

     public void TrapCounter()
    {
        if (trapammo >0)
        {
            Instantiate(gameObject);
            trapammo--;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            
        }

    }

}
