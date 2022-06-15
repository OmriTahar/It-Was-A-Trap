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
    [Tooltip("Cancles the leap if the player flee beyond this range.")]
    [SerializeField] float _maxLeapDistance;

    private Vector3 _directionToPlayer;
    private Vector3 _newDestination;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _hasLeaped = false;
    [SerializeField] bool _isMovingBackwards;

    private WaitForSeconds _waitBeforeLeapCoroutine;
    private WaitForSeconds _startLeapLogicCorutine = new WaitForSeconds(0.3f);
    private WaitForSeconds _waitAfterLeapCoroutine;


    protected override void Awake()
    {
        base.Awake();

        _waitBeforeLeapCoroutine = new WaitForSeconds(_waitBeforeLeap);
        _waitAfterLeapCoroutine = new WaitForSeconds(_waitAfterLeap);
    }

    protected override void Update()
    {
        base.Update();
        _animator.SetBool("isMovingBackwards", _isMovingBackwards);
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

            if (!_hasLeaped)
            {
                if (!_isPlayerInAttackRange)
                {
                    _isMovingBackwards = false;
                    ChasePlayer();
                }
                else if (_isPlayerInAttackRange && _isPlayerTooClose)
                {
                    CreateLeapRange();
                }

                if (!_isPlayerTooClose && _isPlayerInAttackRange)
                {
                    StartCoroutine(Leap());
                }
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

        yield return _waitBeforeLeapCoroutine;
        if ((_playerTransform.position - transform.position).magnitude <= _maxLeapDistance)
        {
            _animator.SetTrigger("Leap");

            yield return _startLeapLogicCorutine;
            if (!IsStunned && _playerTransform != null)
            {
                AttackPrefab.SetActive(true);
                _rb.AddForce((_playerTransform.position - transform.position) * _leapPower, ForceMode.Impulse);
                _isMovingBackwards = true;
            }

            yield return _waitAfterLeapCoroutine;
            if (!IsStunned && _playerTransform != null)
            {
                AttackPrefab.SetActive(false);
                _hasLeaped = false;
            }
        }
        else // Leap distance is too long
        {
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
