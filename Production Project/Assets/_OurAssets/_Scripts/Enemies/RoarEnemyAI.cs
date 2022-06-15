using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RoarEnemyAI : BaseEnemyAI
{
    [Header("Ranged Attack Settings")]
    [SerializeField] Transform _shoutShootPoint;
    [SerializeField] float _shoutForce;
    private ShoutsPool _shoutsPool;

    [Header("Fleeing")]
    [SerializeField] float _startFleeFromPlayer_Range;
    [SerializeField] float _fleeingDuration;
    [SerializeField] float _fleeingDistance;
    private Vector3 _directionToPlayer;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    [SerializeField] bool _isFleeing;

    private WaitForSeconds _fleeDurationCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _shoutsPool = GetComponent<ShoutsPool>();
        _fleeDurationCoroutine = new WaitForSeconds(_fleeingDuration);
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

            if (!_isPlayerInAttackRange && !_isFleeing)
            {
                ChasePlayer();
            }

            if (!_isFleeing)
            {
                if (IsEnemyActivated && _isPlayerTooClose && _canFlee)
                {
                    StartCoroutine(Flee());
                }

                if (IsEnemyActivated && (!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
                {
                    RangeAttack();
                }
            }
        }
    }

    protected override void ChasePlayer()
    {
        base.ChasePlayer();
    }

    private void RangeAttack()
    {
        _agent.SetDestination(transform.position); // Make sure enemy doesn't move while attacking
        transform.LookAt(_playerTransform);

        if (!_isAlreadyAttacked)
        {
            GameObject shout = _shoutsPool.GetShoutFromPool();
            shout.transform.position = _shoutShootPoint.position;
            shout.transform.rotation = Quaternion.identity;

            Rigidbody rb = shout.GetComponent<Rigidbody>();
            rb.AddForce((_playerTransform.position - shout.transform.position).normalized * _shoutForce, ForceMode.Impulse);

            _isAlreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
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
        {
            return false;
        }
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
