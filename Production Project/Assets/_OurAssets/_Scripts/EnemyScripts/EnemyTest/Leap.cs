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
            _hasAttacked = true;
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
            print("Leaped player!");
            _hasAttacked = false;
        }
        else
        {
            print("Else!");
        }
    }
}
