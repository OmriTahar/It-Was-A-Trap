using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IAttackable<Unit>
{

    [Header("Attack Settings")]
    [SerializeField] protected int _damage;

    [Header("Stun Settings")]
    [SerializeField] protected bool _causeStun = false;
    [SerializeField] protected float _stunDuration = 2f;

    protected Unit _attackedUnit;
    protected PlayerController _playerController;
    protected WaitForSeconds _stunEndCoroutine;

    void Awake()
    {
        _stunEndCoroutine = new WaitForSeconds(_stunDuration);
    }

    void IAttackable<Unit>.Attack(Unit unit)
    {
        unit._unitHP -= _damage;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _attackedUnit = other.gameObject.GetComponent<Unit>();
            _attackedUnit.RecieveDamage(this);

            if (_causeStun)
            {
                StartCoroutine(StunPlayer(other));
            }
        }
    }

    protected IEnumerator StunPlayer(Collider other)
    {
        _attackedUnit.IsStunned = true;
        _playerController = other.gameObject.GetComponent<PlayerController>();
        _playerController.PlayerCanMove = false;

        yield return _stunEndCoroutine;
        _attackedUnit.IsStunned = false;
        _playerController.PlayerCanMove = true;
    }
}