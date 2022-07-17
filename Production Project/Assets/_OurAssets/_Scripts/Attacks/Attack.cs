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

    private bool _hasAlreadyStunned = false;

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
            _attackedUnit.RecieveDamage(this, false);

            if (_causeStun)
            {
                StartCoroutine(StunPlayer(other, _attackedUnit));
            }
        }
    }

    protected IEnumerator StunPlayer(Collider other, Unit attackedUnit)
    {
        if (!_hasAlreadyStunned)
        {
            _hasAlreadyStunned = true;
            attackedUnit.IsStunned = true;

            _playerController = other.gameObject.GetComponent<PlayerController>();
            GameManager.Instance.IsPlayerActive(false);
        }

        yield return new WaitForSeconds(_stunDuration);
        attackedUnit.IsStunned = false;
        _hasAlreadyStunned = false;
        GameManager.Instance.IsPlayerActive(true);
    }
}