using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] int maxHP;
    private int currentHP;
    private WallsPool _wallPool;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        if (currentHP <= 0)
            _wallPool.ReturnWallToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            GetHit();
    }

    private void GetHit()
    {
        currentHP--;
    }

    public void SetMe(WallsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _wallPool = myPool;
    }

    public void ReturnToPool()
    {
        _wallPool.ReturnWallToPool(gameObject);
    }
}