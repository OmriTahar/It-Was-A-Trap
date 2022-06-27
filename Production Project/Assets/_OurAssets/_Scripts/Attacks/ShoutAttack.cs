using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShoutAttack : Attack
{

    [Header("Refrences")]
    [SerializeField] ParticleSystem _attackEffect;

    [Header("Shout Settings")]
    [SerializeField] float _shoutFieldActiveDuration = 0.5f;

    [Header("Colors Settings")]
    [SerializeField] float _colorLerpTotalDuration;
    [SerializeField] Color _chargeColor = new Color(100, 0, 0);
    [SerializeField] Color _activationColor = new Color(255, 0, 0);

    private Collider MyCollider;
    private MeshRenderer MyRenderer;
    private float _colorLerpElapsedTime = 0f;
    private bool _startAttackLogic = false;
    private bool _alreadyAttacked;

    private void Awake()
    {
        MyCollider = GetComponent<Collider>();
        MyRenderer = GetComponent<MeshRenderer>();

        MyRenderer.material.color = _chargeColor;
        MyRenderer.enabled = false;
        MyCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_startAttackLogic)
        {
            if (other.CompareTag("Player") && !_alreadyAttacked)
            {
                _alreadyAttacked = true;
                _attackedUnit = other.gameObject.GetComponent<Unit>();
                _attackedUnit.RecieveDamage(this);

                if (_causeStun)
                    StartCoroutine(StunPlayer(other, _attackedUnit));
            }
        }
    }

    public void ActivateShout()
    {
        StartCoroutine(ShoutCoroutine());
    }

    private IEnumerator ShoutCoroutine()
    {
        MyRenderer.enabled = true;
        MyRenderer.material.color = _chargeColor;

        while (_colorLerpElapsedTime < _colorLerpTotalDuration)
        {
            _colorLerpElapsedTime += Time.deltaTime;
            MyRenderer.material.color = Color.Lerp(_chargeColor, _activationColor, _colorLerpElapsedTime / _colorLerpTotalDuration);
            yield return null;
        }
        _colorLerpElapsedTime = 0f;

        if (_attackEffect) _attackEffect.Play();
        MyCollider.enabled = true;
        MyRenderer.enabled = false;
        _startAttackLogic = true;

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(_shoutFieldActiveDuration);

        _startAttackLogic = false;
        MyCollider.enabled = false;
        _alreadyAttacked = false;
        MyRenderer.material.color = _chargeColor;
    }
}
