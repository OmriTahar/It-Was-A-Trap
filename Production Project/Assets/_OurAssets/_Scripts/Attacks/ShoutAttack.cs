using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutAttack : Attack
{

    private BoxCollider _myCollider;
    [SerializeField] ShoutsPool _shoutsPool;
    [SerializeField] float _decayTime = 3f;

    private bool _alreadyAttacked;
    private bool _stunnedPlayer;

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
                _stunnedPlayer = true;
                StartCoroutine(StunPlayer(other));
                StartCoroutine(Decay(_stunDuration + 0.1f));
            }
            else
                _shoutsPool.ReturnProjectileToPool(gameObject);
        }
        else
        {
            StartCoroutine(Decay(_stunDuration * 2));
        }

    }

    private IEnumerator Decay(float decayTime)
    {
        yield return new WaitForSeconds(decayTime);
        print("Shout decayed");
        _shoutsPool.ReturnProjectileToPool(gameObject);
    }

    public void SetMe(ShoutsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _shoutsPool = myPool;
    }
}
