using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IAttackable<Unit>
{
    [SerializeField] protected int _damage;
    [SerializeField] protected bool _stunning = false;

    //[SerializeField] protected GameObject _hitEffect;
    //protected Transform _hitTransform;

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
    }

    //public virtual void PlayHitEffect(GameObject hitEffect, Vector3 hitTransform)
    //{
    //    if (_hitEffect != null)
    //    {
    //        Instantiate(hitEffect,hitTransform, hitTransform);
    //        print("Instansiated Effect!");

    //        //_hitEffect.transform.position = hitPosition;
    //        //_hitEffect.SetActive(true);
    //    }
    //}
}