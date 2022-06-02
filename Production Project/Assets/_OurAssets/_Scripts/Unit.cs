using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("Unit Settings")]
    public float UnitHP;
    public float UnitMaxHP;
    public float UnitAttackRange;
    [SerializeField] Image _healthBarBG, _healthBar;

    private void Awake()
    {
        UnitHP = UnitMaxHP;

        if (_healthBar)
            _healthBar.fillAmount = UnitHP / UnitMaxHP;
    }

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        enemy.Attack(this);

        if (_healthBar)
        {
            _healthBar.fillAmount = UnitHP / UnitMaxHP;
            //print("Fill Amount: " + _healthBar.fillAmount);
        }

        CheckDeath();
    }

    void CheckDeath()
    {
        if (UnitHP <= 0)
        {
            print($"{gameObject.name} died");
            Destroy(gameObject);
        }
    }

}