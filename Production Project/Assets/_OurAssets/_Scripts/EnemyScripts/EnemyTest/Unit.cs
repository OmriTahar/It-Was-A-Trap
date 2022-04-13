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
        enemy.Attack(this);

        if (_myHealthSlider != null)
            _myHealthSlider.value = CalculateHealth();

    }


    float CalculateHealth()
    {
        return _unitHP / _unitMaxHP;
    }

}