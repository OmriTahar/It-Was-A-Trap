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
    [SerializeField] Transform _shoutShootPoint;

    [Header("Shout Attack Settings")]
    [Tooltip("Cannot change while in editor runtime")]
    [SerializeField] [Range(2,5)] float _waitAfterShout;

    [Header("Fleeing")]
    [SerializeField] float _startFleeFromPlayer_Range;
    [SerializeField] float _fleeingDuration;

    [Header("Enemy Status")]
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    [SerializeField] bool _isFleeing;

    private bool _isShouting;
    private Vector3 _directionToPlayer;
    private WaitForSeconds _fleeDurationCoroutine;
    private WaitForSeconds _waitAfterShoutCoroutine;


    protected override void Awake()
    {
        base.Awake();
        _attackZone.SetActive(true);
        #region Coroutines Cacheing
        _fleeDurationCoroutine = new WaitForSeconds(_fleeingDuration);
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

                if (!_isFleeing)
                {
                    if (!_isPlayerInAttackRange)
                    {
                        ChasePlayer();
                    }

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

    private void AttemptShout()
    {
        transform.LookAt(_playerTransform);

        _isAlreadyAttacked = true;
        _isShouting = true;

        StartCoroutine(Shout(_playerTransform.position));
    }

    IEnumerator Shout(Vector3 lockedPlayerPosition)
    {
        _animator.SetTrigger("Shout");
        _shoutAttack.ActivateShout();

        yield return _waitAfterShoutCoroutine;
        _isShouting = false;
        transform.LookAt(_playerTransform);

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
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/Bunny/Bunny Starting Stun Attack");
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
