using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShoutAttack : Attack
{

    [Header("Refrences")]
    [SerializeField] GameObject _attackEffect;

    [Header("Shout Settings")]
    [SerializeField] float _shoutFieldActiveDuration = 0.5f;

    [Header("Colors Settings")]
    [SerializeField] float _colorLerpTotalDuration;
    [SerializeField] Color _chargeColor = new Color(100, 0, 0);
    [SerializeField] Color _activationColor = new Color(255, 0, 0);

    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;
    private float _colorLerpElapsedTime = 0f;
    private bool _startAttackLogic = false;
    private bool _alreadyAttacked;

    private void Awake()
    {
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.material.color = _chargeColor;
        _meshRenderer.enabled = false;
        _meshCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_startAttackLogic)
        {
            if (other.CompareTag("Player") && !_alreadyAttacked)
            {
                _alreadyAttacked = true;
                _attackedUnit = other.gameObject.GetComponent<Unit>();

                _attackedUnit.RecieveDamage(this, false);

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
        _meshRenderer.enabled = true;
        _meshRenderer.material.color = _chargeColor;

        while (_colorLerpElapsedTime < _colorLerpTotalDuration)
        {
            _colorLerpElapsedTime += Time.deltaTime;
            _meshRenderer.material.color = Color.Lerp(_chargeColor, _activationColor, _colorLerpElapsedTime / _colorLerpTotalDuration);
            yield return null;
        }
        _colorLerpElapsedTime = 0f;

        if (_attackEffect)
            _attackEffect.SetActive(true);

        _meshCollider.enabled = true;
        _meshRenderer.enabled = false;
        _startAttackLogic = true;

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(_shoutFieldActiveDuration);

        _attackEffect.SetActive(false);
        _startAttackLogic = false;
        _meshCollider.enabled = false;
        _alreadyAttacked = false;
        _meshRenderer.material.color = _chargeColor;
    }
}
