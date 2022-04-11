using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotProjectile : Attack
{

    [SerializeField] ProjectilePool _projectilePool;

    private void Start()
    {
        _projectilePool = GetComponentInParent<ProjectilePool>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this);
        }

        _projectilePool.ReturnProjectileToPool(gameObject);
    }
}
