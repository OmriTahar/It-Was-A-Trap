using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangedEnemyAI : BaseEnemyAI
{

    [Header("Ranged Attack Settings")]
    [SerializeField] Transform _rangedShootPoint;
    [SerializeField] float _rangedShootForce;
    private ProjectilePool _rangedProjectilePool;

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


    protected override void Awake()
    {
        base.Awake();
        _rangedProjectilePool = GetComponent<ProjectilePool>();
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
            #region Chase

            if (!_isPlayerInAttackRange && !_isFleeing)
            {
                ChasePlayer();
                print(name + " is Chasing");
            }

            #endregion

            if (!_isFleeing)
            {
                if (IsEnemyActivated && _isPlayerTooClose && _canFlee)
                {
                    StartCoroutine(Flee());
                    print(name + " is Fleeing");
                }

                if (IsEnemyActivated && (!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
                {
                    RangeAttack();
                    print(name + " is Attacking");
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
            GameObject projectile = _rangedProjectilePool.GetProjectileFromPool();
            projectile.transform.position = _rangedShootPoint.position;
            projectile.transform.rotation = Quaternion.identity;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce((_playerTransform.position - projectile.transform.position).normalized * _rangedShootForce, ForceMode.Impulse);

            _isAlreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private bool CanEnemyFlee()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * 10;
        Vector3 newFleePosition = transform.position + _directionToPlayer;
        print("new Flee Position Check: " + newFleePosition);

        if (_agent.CalculatePath(newFleePosition, new NavMeshPath()))
        {
            print(name + " CAN flee");
            return true;
        }
        else
        {
            print(name + " can NOT flee");
            return false;
        }
    }

    private IEnumerator Flee()
    {
        _isFleeing = true;

        _directionToPlayer = (transform.position - _playerTransform.position).normalized * 10;
        Vector3 newFleePosition = transform.position + _directionToPlayer;

        print("New Set Destination: " + newFleePosition);

        _agent.SetDestination(newFleePosition);

        yield return new WaitForSeconds(_fleeingDuration);

        print("Finished FLEEING!");
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
