using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : Attack
{

    bool _hasAttacked = false;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_hasAttacked)
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
            print("Leaped player!");
            _hasAttacked = true;
        }
        else
        {
            print("Else!");
        }
    }
}
