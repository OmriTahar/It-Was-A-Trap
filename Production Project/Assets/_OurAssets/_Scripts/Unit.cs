using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] internal float _unitHP, _unitMaxHP, _unitRange;
    [SerializeField] Slider _myHealthSlider;

    private void Start()
    {
        _unitHP = _unitMaxHP;

        if (_myHealthSlider != null)
            _myHealthSlider.value = ChangeHealthUI();

    }

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        enemy.Attack(this);

        if (_myHealthSlider != null)
            _myHealthSlider.value = ChangeHealthUI();

        CheckDeath();
    }

    float ChangeHealthUI()
    {
        return _unitHP / _unitMaxHP;
    }

    void CheckDeath()
    {
        if (_unitHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        print("HP: " + _unitHP + ". Im dying!");
        Destroy(gameObject);
    }
}