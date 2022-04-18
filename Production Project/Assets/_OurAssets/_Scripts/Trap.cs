using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    int trapdmg = 50;
    EnemyAI aI;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            aI._unitHP -= trapdmg;
            if (aI._unitHP >= 0)
            {
                Destroy(aI);
            }
        }
    }
}
