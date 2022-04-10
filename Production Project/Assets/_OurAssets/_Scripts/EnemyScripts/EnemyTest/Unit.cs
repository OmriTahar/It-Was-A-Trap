using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] internal int UnitHP;
    [SerializeField] internal int UnitRange;

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        enemy.Attack(this);
    }
}