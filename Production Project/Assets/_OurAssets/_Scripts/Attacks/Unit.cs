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
    public static event Action OnPlayerKilled;

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

    private void CheckDeath()
    {
        if (_unitHP <= 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        if (gameObject.CompareTag("Player"))
        {
            PlayerData.Instance.AnimatorGetter.Play("Death", 0);
        }
        else
        {
            PlayerData.Instance.AddScore();
            OnBunnyKilled?.Invoke();
            Destroy(gameObject);
        }
    }

    public void PlayerDeath() // Executed after death animation end
    {
        OnPlayerKilled?.Invoke();
    }
}