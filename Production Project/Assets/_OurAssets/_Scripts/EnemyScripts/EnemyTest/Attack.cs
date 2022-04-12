using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IAttackable<Unit>
{
    [SerializeField] protected int _damage;
    [SerializeField] protected bool _stunning = false;

    void IAttackable<Unit>.Attack(Unit unit)
    {
        unit._unitHP -= _damage;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
        }

        //Destroy(gameObject);
    }

}