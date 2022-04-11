using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotProjectile : Attack
{

    private BoxCollider _myCollider;
    [SerializeField] ProjectilePool _projectilePool;
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
            _projectilePool.ReturnProjectileToPool(gameObject);
        }
        else
        {
            _myCollider.isTrigger = false;
            StartCoroutine(Decay());
        }
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);
        _projectilePool.ReturnProjectileToPool(gameObject);
    }

    public void SetMe(ProjectilePool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _projectilePool = myPool;
    }
}
