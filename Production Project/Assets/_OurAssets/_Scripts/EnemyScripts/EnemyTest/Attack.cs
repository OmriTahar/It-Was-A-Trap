using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IAttackable<Unit>
{
    [SerializeField] protected int Damage;
    [SerializeField] protected bool Stunning = false;

    void IAttackable<Unit>.Attack(Unit unit)
    {
        unit.UnitHP -= Damage;
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