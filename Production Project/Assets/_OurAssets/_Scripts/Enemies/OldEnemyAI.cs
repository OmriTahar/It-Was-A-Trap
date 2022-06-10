using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class OldEnemyAI : Unit
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    [Header("General References")]
    [SerializeField] Transform _playerTransform;
    [SerializeField] LayerMask _groundLayer, _playerLayer;

    [Header("Enemy TYPE")]
    public bool IsRangedEnemy;
    public bool IsLeaper;

    [Header("Chase Settings")]
    [SerializeField] float SlerpCurve;

    [Header("General Attack Settings")]
    [SerializeField] GameObject AttackPrefab;
    [SerializeField] float _timeBetweenAttacks;

    [Header("Leaper Settings")]
    [SerializeField] float _leapPower;
    [SerializeField] float _waitBeforeLeap;
    [SerializeField] float _waitAfterLeap;
    [SerializeField] bool _hasLeaped = false;
    [SerializeField] float _leaperJumpPower;

    [Header("Ranged Attack Settings")]
    [SerializeField] Transform _rangedShootPoint;
    [SerializeField] float _rangedShootForce;
    private ProjectilePool _rangedProjectilePool;

    [Header("Fleeing")]
    [SerializeField] float _startFleeFromPlayer_Range;
    [SerializeField] float _fleeingDuration;
    [SerializeField] float _fleeingDistance;
    private Vector3 _directionToPlayer;

    [Header("Stun Settings")]
    public bool IsStunned;
    [SerializeField] ParticleSystem _stunEffect;

    [Header("Enemy Status")]
    public bool IsEnemyActivated;
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    [SerializeField] bool _isFleeing;

    #region UnUsedVariables

    // ---------- This was before manual enemy activation ----------
    //_isPlayerInSight = Physics.CheckSphere(transform.position, _playerSightRange, _playerLayer);

    //[Header("Patroling")]
    private bool _isDestinationSet;
    private Vector3 _moveDestination;
    private float _PatrolingPointRange;
    //[SerializeField] bool _isPlayerInSight;
    //[SerializeField] float _playerSightRange;

    //[Header("Jump Settings")]
    //[SerializeField] Transform _groundCheck;
    //[SerializeField] float _groundCheckRadius;
    //[SerializeField] float _jumpForce;
    //[SerializeField] bool _isGrounded;

    //[SerializeField] float _jumpVelocity;
    //[SerializeField] float _gravity = 9.81f;
    //[SerializeField] float _gravityScale = 5f;

    #endregion


    private void Awake()
    {
        LockEnemyType();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _rangedProjectilePool = GetComponent<ProjectilePool>();
    }

    private void Update()
    {
        PlayerDetaction();
        EnemyStateMachine();
    }

    private void LockEnemyType()
    {
        if (IsRangedEnemy || (IsRangedEnemy && IsLeaper))
        {
            IsRangedEnemy = true;
            IsLeaper = false;
        }
        else if (!IsRangedEnemy || (!IsRangedEnemy && !IsLeaper))
        {
            IsRangedEnemy = false;
            IsLeaper = true;
        }
    }

    private void PlayerDetaction()
    {
        if (IsEnemyActivated)
        {
            if (!_isFleeing)
            {
                _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _unitAttackRange, _playerLayer);

                if (IsRangedEnemy)
                {
                    _isPlayerTooClose = Physics.CheckSphere(transform.position, _startFleeFromPlayer_Range, _playerLayer);

                    if (_isPlayerTooClose)
                        _canFlee = CanEnemyFlee();
                }
            }
        }
    }

    private void EnemyStateMachine()
    {

        if (!IsEnemyActivated)
        {
            print(name + " is not activated.");
            _agent.SetDestination(transform.position); // Make sure enemy doesn't move
        }

        else
        {
            #region Stun

            if (IsStunned && _stunEffect != null && !_stunEffect.isPlaying)
            {
                _stunEffect.Play();
                Stunned();
            }
            else if (!IsStunned && _stunEffect.isPlaying)
                _stunEffect.Stop();

            #endregion

            #region Chase

            if (IsEnemyActivated && !_isPlayerInAttackRange && !_isFleeing)
            {
                ChasePlayer();
                print(name + " is Chasing");
            }

            #endregion

            #region Ranged Enemy

            if (IsRangedEnemy)
            {
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

            #endregion

            #region Leaper

            else if (IsLeaper)
            {
                transform.LookAt(_playerTransform);

                if (IsEnemyActivated && _isPlayerInAttackRange && !_hasLeaped)
                {
                    StartCoroutine(Leap());
                    print(name + " is Leaping!");
                }
                else if (IsEnemyActivated && _isPlayerInAttackRange && _hasLeaped)
                {
                    print(name + " is Recharging Leap");
                }
            }

            #endregion
        }
    }

    private void ChasePlayer()
    {
        //var destination = Vector3.Slerp(transform.position, _playerTransform.forward, SlerpCurve); ---- Flank attemp
        _agent.SetDestination(_playerTransform.position);
    }

    private IEnumerator Leap()
    {
        _hasLeaped = true;
        _agent.SetDestination(transform.position);

        yield return new WaitForSeconds(_waitBeforeLeap);
        if (!IsStunned)
        {
            AttackPrefab.SetActive(true);

            _rb.AddForce(new Vector3(0, _leaperJumpPower, 0f), ForceMode.Impulse);
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

    private void Stunned()
    {
        _rb.freezeRotation = true;
        _rb.Sleep(); // Freeze Position
        _agent.SetDestination(transform.position);
    }

    private void ResetAttack()
    {
        _isAlreadyAttacked = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _startFleeFromPlayer_Range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _unitAttackRange);
    }

    #region UnusedMethods
    private void Patroling()
    {
        if (!_isDestinationSet) SearchWalkPoint();

        if (_isDestinationSet)
            _agent.SetDestination(_moveDestination);

        Vector3 distanceToWalkPoint = transform.position - _moveDestination;

        // Destination reached
        if (distanceToWalkPoint.magnitude < 1f)
            _isDestinationSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-_PatrolingPointRange, _PatrolingPointRange);
        float randomX = Random.Range(-_PatrolingPointRange, _PatrolingPointRange);

        _moveDestination = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(_moveDestination, -transform.up, 2f, _groundLayer))
            _isDestinationSet = true;
    }

    #endregion
}