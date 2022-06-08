using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LeaperAI : BaseEnemyAI
{

    [Header("Leap Settings")]
    [SerializeField] float _leapPower;
    [SerializeField] float _waitBeforeLeap;
    [SerializeField] float _waitAfterLeap;
    [SerializeField] float _playerIsTooCloseRange;

    private Vector3 _directionToPlayer;
    private Vector3 _newDestination;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _hasLeaped = false;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void PlayerDetaction()
    {
        if (IsEnemyActivated)
        {
            _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _unitAttackRange, _playerLayer);
            _isPlayerTooClose = Physics.CheckSphere(transform.position, _playerIsTooCloseRange, _playerLayer);
        }
    }

    protected override void EnemyStateMachine()
    {
        base.EnemyStateMachine();

        if (IsEnemyActivated)
        {
            transform.LookAt(_playerTransform);

            if (!_isPlayerInAttackRange)
            {
                ChasePlayer();
                print(name + " is Chasing");
            }
            else if (_isPlayerInAttackRange && _isPlayerTooClose)
            {
                CreateLeapRange();
            }


            if (!_isPlayerTooClose && _isPlayerInAttackRange && !_hasLeaped)
            {
                StartCoroutine(Leap());
                print(name + " is Leaping!");
            }

            if (_hasLeaped)
            {
                print(name + " is Recharging Leap");
            }
        }
    }

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
    }

    private void CreateLeapRange()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * 3;
        _newDestination = transform.position + _directionToPlayer;
        _agent.SetDestination(_newDestination);
    }

    private IEnumerator Leap()
    {
        _hasLeaped = true;
        _agent.SetDestination(transform.position);

        yield return new WaitForSeconds(_waitBeforeLeap);
        if (!IsStunned)
        {
            AttackPrefab.SetActive(true);
            _rb.AddForce((_playerTransform.position - transform.position) * _leapPower, ForceMode.Impulse);
            print("LEAPING!");
        }

        yield return new WaitForSeconds(_waitAfterLeap);
        if (!IsStunned)
        {
            AttackPrefab.SetActive(false);
            _hasLeaped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerIsTooCloseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _unitAttackRange);
    }
}
