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
        print("Shout hurt somthing!");

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
            //_myCollider.isTrigger = false;
            _shoutsPool.ReturnProjectileToPool(gameObject);

            print("Shout hurt player!");

            //PlayHitEffect(_hitEffect, _hitTransform);
        }
        else
        {
            StartCoroutine(Decay());
        }

    }

    private IEnumerator Decay()
    {
        _myCollider.isTrigger = false;
        yield return new WaitForSeconds(_decayTime);
        _shoutsPool.ReturnProjectileToPool(gameObject);
        _myCollider.isTrigger = true;
    }

    public void SetMe(ShoutsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _shoutsPool = myPool;
    }
}
