using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    [Header("Unit Settings")]
    [SerializeField][ReadOnlyInspector] internal float _unitHP = 0;
    [SerializeField] protected float _unitMaxHP, _unitAttackRange;
    [SerializeField] Image _healthBarBG, _healthBar;

    [Header("Stun Settings")]
    public bool IsStunned;
    public ParticleSystem _stunEffect;

    public static event Action OnBunnyKilled;

    // Animation Performance
    int _gotHitHash;


    protected virtual void Start()
    {
        _unitHP = _unitMaxHP;

        if (_healthBar)
            _healthBar.fillAmount = _unitHP / _unitMaxHP;

        if (gameObject.CompareTag("Player"))
            _gotHitHash = Animator.StringToHash("GotHit");
    }

    public void RecieveDamage(IAttackable<Unit> enemy, bool showGotHitAnimation = true)
    {
        enemy.Attack(this);

        if (_healthBar)
            _healthBar.fillAmount = _unitHP / _unitMaxHP;

        if (gameObject.CompareTag("Player") && !IsStunned && showGotHitAnimation)
            PlayerData.Instance.AnimatorGetter.Play(_gotHitHash, 2);

        CheckDeath();
    }

    protected virtual void OnDeath()
    {
        PlayerData.Instance.AddScore();
        OnBunnyKilled?.Invoke();
        Destroy(gameObject);
    }

    private void CheckDeath()
    {
        if (_unitHP <= 0)
        {
            OnDeath();
        }
    }
}