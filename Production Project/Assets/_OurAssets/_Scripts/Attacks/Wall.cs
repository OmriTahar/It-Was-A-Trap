using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    [SerializeField] int maxHP;
    private int CurrentHP;
    private WallsPool _wallPool;


    private void Awake()
    {
        RestartWall();
    }

    private void Update()
    {
        if (CurrentHP <= 0)
            ReturnMySelfToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            GetHit();
    }

    private void GetHit()
    {
        CurrentHP--;
    }

    public void SetMe(WallsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _wallPool = myPool;
    }

    public void ReturnMySelfToPool(GameObject returningObject)
    {
        _wallPool.ReturnWallToPool(returningObject);
        RestartWall();
    }

    public void RestartWall()
    {
        CurrentHP = maxHP;
    }
}