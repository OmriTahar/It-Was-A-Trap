using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RoarEnemyAI : BaseEnemyAI
{

    [Header("Refrences")]
    [SerializeField] GameObject _attackZone;
    [SerializeField] ShoutAttack _shoutAttack;
    
    [Header("Shout Attack Settings")]
    [SerializeField] Transform _shoutShootPoint;
    [SerializeField] float _shoutForce;
    [SerializeField] float _waitBeforeShout;
    [SerializeField] float _delayAfterStartingShoutAnimation;
    [SerializeField] float _waitAfterShout = 1.2f;

    [Header("Fleeing")]
    [SerializeField] float _startFleeFromPlayer_Range;
    [SerializeField] float _fleeingDuration;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    [SerializeField] bool _isFleeing;

    private bool _isShouting;
    private Vector3 _directionToPlayer;
    private WaitForSeconds _fleeDurationCoroutine;
    private WaitForSeconds _delayAfterStartingShoutAnimationCoroutine;
    private WaitForSeconds _waitBeforeShoutCoroutine;
    private WaitForSeconds _waitAfterShoutCoroutine;


    protected override void Awake()
    {
        base.Awake();
        _attackZone.SetActive(false);
        #region Coroutines Cacheing
        _fleeDurationCoroutine = new WaitForSeconds(_fleeingDuration);
        _delayAfterStartingShoutAnimationCoroutine = new WaitForSeconds(_delayAfterStartingShoutAnimation);
        _waitBeforeShoutCoroutine = new WaitForSeconds(_waitBeforeShout);
        _waitAfterShoutCoroutine = new WaitForSeconds(_waitAfterShout);
        #endregion
    }

    protected override void PlayerDetaction()
    {
        if (IsEnemyActivated)
        {
            if (!_isFleeing)
            {
                _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _unitAttackRange, _playerLayer);
                _isPlayerTooClose = Physics.CheckSphere(transform.position, _startFleeFromPlayer_Range, _playerLayer);

                if (_isPlayerTooClose)
                    _canFlee = CanEnemyFlee();
            }
        }
    }

    protected override void EnemyStateMachine()
    {
        base.EnemyStateMachine();

        if (IsEnemyActivated)
        {
            if (_isShouting)
            {
                _agent.SetDestination(transform.position);
                _rb.Sleep();
            }
            else
            {
                if (!_isPlayerInAttackRange && !_isFleeing)
                {
                    ChasePlayer();
                }

                if (!_isFleeing)
                {
                    if (_isPlayerTooClose && _canFlee)
                    {
                        StartCoroutine(Flee());
                    }

                    if ((!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
                    {
                        if (!_isAlreadyAttacked)
                        {
                            AttemptShout();
                        }
                    }
                }
            }
        }
    }

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
        transform.LookAt(_playerTransform);
    }

    private void AttemptShout()
    {
        _isShouting = true;
        _isAlreadyAttacked = true;

        transform.LookAt(_playerTransform);
        _attackZone.SetActive(true);

        StartCoroutine(Shout(_playerTransform.position));
    }

    IEnumerator Shout(Vector3 lockedPlayerPosition)
    {
        yield return _waitBeforeShoutCoroutine;
        _animator.SetTrigger("Shout");

        yield return _delayAfterStartingShoutAnimationCoroutine;
        _shoutAttack.ActivateShout();

        yield return _waitAfterShoutCoroutine;
        _isShouting = false;
        _attackZone.SetActive(false);

        Invoke(("ResetAttack"), _timeBetweenAttacks);
    }

    private bool CanEnemyFlee()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * 10;
        Vector3 newFleePosition = transform.position + _directionToPlayer;

        if (_agent.CalculatePath(newFleePosition, new NavMeshPath()))
        {
            return true;
        }
        else
            return false;
    }

    private IEnumerator Flee()
    {
        _isFleeing = true;
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * 10;
        Vector3 newFleePosition = transform.position + _directionToPlayer;
        _agent.SetDestination(newFleePosition);

        yield return _fleeDurationCoroutine;
        _isFleeing = false;
    }

    private void ResetAttack()
    {
        _isAlreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _startFleeFromPlayer_Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _unitAttackRange);
    }
}
