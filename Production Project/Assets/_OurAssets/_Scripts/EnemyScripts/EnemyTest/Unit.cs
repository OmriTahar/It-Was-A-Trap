using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    [SerializeField] internal int _unitHP;
    [SerializeField] internal int _unitMaxHP;
    [SerializeField] internal int _unitRange;

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