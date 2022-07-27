using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotProjectile : Attack
{

    [Header("Carrot Settings")]
    [SerializeField] float _decayTime = 3f;
    [SerializeField] GameObject _myEffect;

    private BoxCollider _myCollider;
    private ProjectilePool _projectilePool;


    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        _myEffect.SetActive(true);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Unit>().RecieveDamage(this, true);
        }

        _myCollider.isTrigger = false;
        _myEffect.SetActive(false);

        StartCoroutine(Decay());
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);
        _projectilePool.ReturnProjectileToPool(gameObject);
        _myCollider.isTrigger = true;
    }

    public void SetMe(ProjectilePool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _projectilePool = myPool;
    }
}
