using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] int maxHP;
    int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        if (currentHP <= 0)
            Destroy(gameObject);
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

}