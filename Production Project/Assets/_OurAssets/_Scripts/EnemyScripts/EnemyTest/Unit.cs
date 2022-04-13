using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    [SerializeField] internal float _unitHP;
    [SerializeField] internal float _unitMaxHP;
    [SerializeField] internal float _unitRange;

    [SerializeField] Slider _myHealthSlider;

    private void Start()
    {
        _unitHP = _unitMaxHP;

        if (_myHealthSlider != null)
            _myHealthSlider.value = CalculateHealth();

    }

    public void RecieveDamage(IAttackable<Unit> enemy)
    {
        print("Slider Value Before: " + _myHealthSlider.value + " Player Health: " + _unitHP);

        enemy.Attack(this);

        if (_myHealthSlider != null)
            _myHealthSlider.value = CalculateHealth();

        print("Slider Value After: " + _myHealthSlider.value + " Player Health: " + _unitHP);
    }


    float CalculateHealth()
    {
        return _unitHP / _unitMaxHP;
    }
}