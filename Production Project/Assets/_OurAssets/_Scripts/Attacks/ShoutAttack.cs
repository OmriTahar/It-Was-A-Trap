using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutAttack : Attack
{

    private BoxCollider _myCollider;
    [SerializeField] ShoutsPool _shoutsPool;
    [SerializeField] float _decayTime = 3f;

    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
            //PlayHitEffect(_hitEffect, _hitTransform);
        }

        _myCollider.isTrigger = false;
        StartCoroutine(Decay());
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);
        _shoutsPool.ReturnProjectileToPool(gameObject);
        _myCollider.isTrigger = true;
    }

    public void SetMe(ShoutsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _shoutsPool = myPool;
    }
}
