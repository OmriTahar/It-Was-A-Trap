using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : Unit
{
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    [Header("General Settings & References")]
    [SerializeField] Transform _playerTransform;
    [SerializeField] LayerMask _groundLayer, _playerLayer;
    public bool IsEnemyActivated;
    public float SlerpCurve = 30f;

    [Header("Stun Settings")]
    public bool IsStunned;
    [SerializeField] ParticleSystem _stunEffect;

    [Header("Enemy TYPE")]
    public bool IsRangedEnemy;
    public bool IsLeapEnemy;

    [Header("Attack Settings")]
    [SerializeField] float _timeBetweenAttacks;
    [SerializeField] bool _isPlayerInAttackRange, _isAlreadyAttacked;
    public GameObject AttackPrefab;

    [Header("Ranged Attack")]
    [SerializeField] Transform ShootPoint;
    [SerializeField] float ShootForce;
    private ProjectilePool _projectilePool;

    [Header("Fleeing")]
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _isFleeing;
    [SerializeField] float _fleeingDuration;
    [SerializeField] float _moveBackRange;
    [SerializeField] bool _canFlee;
    private Vector3 _directionToPlayer;

    [Header("Leaper")]
    [SerializeField] float _leapPower;
    [SerializeField] float _waitBeforeLeap;
    [SerializeField] float _waitAfterLeap;
    [SerializeField] bool _hasLeaped = false;
    [SerializeField] float _leaperJumpPower = 3f;

    #region UnUsedVariables

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
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _projectilePool = GetComponent<ProjectilePool>();

        LockEnemyType();
    }

    private void Update()
    {
        PlayerDetaction();
        EnemyStateMachine();
    }

    private void LockEnemyType()
    {
        if (IsRangedEnemy || (IsRangedEnemy && IsLeapEnemy))
        {
            IsLeapEnemy = false;
        }
        else if (!IsRangedEnemy || (!IsRangedEnemy && !IsLeapEnemy))
        {
            IsLeapEnemy = true;
        }
    }

    private void PlayerDetaction()
    {
        //_isPlayerInSight = Physics.CheckSphere(transform.position, _playerSightRange, _playerLayer); // This was before manual enemy activation

        if (IsEnemyActivated)
        {
            _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _unitRange, _playerLayer);

            if (IsRangedEnemy)
            {
                _isPlayerTooClose = Physics.CheckSphere(transform.position, _moveBackRange, _playerLayer);

                if (_isPlayerTooClose)
                    _canFlee = CanEnemyFlee();
            }
        }
    }

    private void EnemyStateMachine()
    {

        if (IsStunned && _stunEffect != null && !_stunEffect.isPlaying)
        {
            _stunEffect.Play();
            Stunned();
        }
        else if (!IsStunned && _stunEffect.isPlaying)
            _stunEffect.Stop();


        if (!IsEnemyActivated)
        {
            print("Enemy: " + name + " is not activated.");
            _agent.SetDestination(transform.position); // Make sure enemy doesn't move
        }
        else
        {
            if (IsEnemyActivated && !_isPlayerInAttackRange && !_isFleeing)
            {
                ChasePlayer();
                print("Chasing");
            }

            if (IsRangedEnemy)
            {
                if (IsEnemyActivated && _isPlayerTooClose && _canFlee)
                {
                    StartCoroutine(Flee());
                    print("fleeing");
                }

                if (IsEnemyActivated && (!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
                {
                    RangeAttack();
                    print("Attacking");
                }
            }
            else if (IsLeapEnemy)
            {
                transform.LookAt(_playerTransform);

                if (IsEnemyActivated && _isPlayerInAttackRange && !_hasLeaped)
                {
                    StartCoroutine(Leap());
                    print("Leaping!");
                }
                else if (IsEnemyActivated && _isPlayerInAttackRange && _hasLeaped)
                {
                    print("Recharging Leap");
                }
            }
        }
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

    private void ChasePlayer()
    {
        //var destination = Vector3.Slerp(transform.position, _playerTransform.forward, SlerpCurve); ---- Flank attemp
        _agent.SetDestination(_playerTransform.position);
    }

    private void RangeAttack()
    {
        _agent.SetDestination(transform.position); // Make sure enemy doesn't move while attacking
        transform.LookAt(_playerTransform);

        if (!_isAlreadyAttacked)
        {
            GameObject projectile = _projectilePool.GetProjectileFromPool();
            projectile.transform.position = ShootPoint.position;
            projectile.transform.rotation = Quaternion.identity;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce((_playerTransform.position - projectile.transform.position).normalized * ShootForce, ForceMode.Impulse);

            _isAlreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private bool CanEnemyFlee()
    {
        _directionToPlayer = transform.position - _playerTransform.position;
        Vector3 newPosition = transform.position + _directionToPlayer;

        if (_agent.CalculatePath(newPosition, new NavMeshPath()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator Flee() // Request specification about fleeing behaviour
    {
        if (!_isFleeing)
        {
            _isFleeing = true;

            // Current Fleeing behaviour
            _directionToPlayer = transform.position - _playerTransform.position;
            Vector3 newPosition = transform.position + _directionToPlayer;
            _agent.SetDestination(newPosition);
            // -------------------------

            yield return new WaitForSeconds(_fleeingDuration);
            _isFleeing = false;
        }
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
        Gizmos.DrawWireSphere(transform.position, _moveBackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _unitRange);
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