using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutAttack : Attack
{

    private BoxCollider _myCollider;
    [SerializeField] ShoutsPool _shoutsPool;
    [SerializeField] float _decayTime = 3f;

    private bool _alreadyAttacked;

    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    public override void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && !_alreadyAttacked)
        {
            _alreadyAttacked = true;
            _attackedUnit = other.gameObject.GetComponent<Unit>();
            _attackedUnit.RecieveDamage(this);

            if (_causeStun)
            {
                StartCoroutine(StunPlayer(other));
                StartCoroutine(Decay(_stunDuration));
            }
            else
            {
                _shoutsPool.ReturnProjectileToPool(gameObject);
            }
        }
        else
        {
            StartCoroutine(Decay(_decayTime));
        }

    }

    private IEnumerator Decay(float decayTime)
    {
        yield return new WaitForSeconds(decayTime);
        _shoutsPool.ReturnProjectileToPool(gameObject);
    }

    public void SetMe(ShoutsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _shoutsPool = myPool;
    }
}
