using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public int _unitHP, _unitMaxHP;
    public float _unitRange;

    [SerializeField] Image _healthBarBG, _healthBar;

    private void Awake()
    {
        _unitHP = _unitMaxHP;

        if (_healthBar)
            _healthBar.fillAmount = _unitHP / _unitMaxHP;
    }

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        enemy.Attack(this);

        if (_healthBar)
            _healthBar.fillAmount = _unitHP / _unitMaxHP;

        CheckDeath();
    }

    void CheckDeath()
    {
        if (_unitHP <= 0)
        {
            print($"{gameObject.name} died");
            Destroy(gameObject);
        }
    }

}