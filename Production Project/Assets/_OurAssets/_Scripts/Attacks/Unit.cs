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
    [SerializeField] Image _healthBar;

    [Header("Stun Settings")]
    public bool IsStunable = false;
    public bool IsStunned;
    public ParticleSystem _stunEffect;

    public static event Action OnBunnyKilled;
    public static event Action OnPlayerKilled;

    int _gotHitAnimationHash;
    int _gotHitEffectHash;
    protected bool IsDead = false;


    protected virtual void Start()
    {
        _unitHP = _unitMaxHP;

        if (_healthBar)
            _healthBar.fillAmount = _unitHP / _unitMaxHP;

        if (gameObject.CompareTag("Player"))
        {
            _gotHitAnimationHash = Animator.StringToHash("GotHit");
            _gotHitEffectHash = Animator.StringToHash("Hit Effect");
        }
    }

    public void RecieveDamage(IAttackable<Unit> enemy, bool showGotHitAnimation = true)
    {
        if (!IsDead)
        {
            enemy.Attack(this);

            if (_healthBar)
                _healthBar.fillAmount = _unitHP / _unitMaxHP;


            if (_unitHP > 0 && gameObject.CompareTag("Player"))
            {
                if (!IsStunned && showGotHitAnimation)
                {
                    PlayerData.Instance.HitEffectAnimator.Play(_gotHitEffectHash, 0);
                    PlayerData.Instance.PlayerAnimatorGetter.Play(_gotHitAnimationHash, 2);
                }
            }

            CheckDeath();
        }
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
        IsDead = true;

        PlayerData.Instance.AddScore();
        OnBunnyKilled?.Invoke();
        Destroy(gameObject);
    }

    public void PlayerDeath() // Executed after death animation end
    {
        OnPlayerKilled?.Invoke();
    }
}