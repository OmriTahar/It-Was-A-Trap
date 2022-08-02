using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : Attack
{
    Collider _myCollider;
    bool _hasAttacked = false;

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _myCollider.enabled = true;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_hasAttacked)
        {
            _hasAttacked = true;
            other.gameObject.GetComponent<Unit>().RecieveDamage(this, true);

            _hasAttacked = false;
            _myCollider.enabled = false;
        }
    }
}
