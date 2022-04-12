using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] internal int _unitHP;
    [SerializeField] internal int _unitRange;

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        enemy.Attack(this);
    }
}