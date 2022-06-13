using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BaseEnemyAI : Unit
{
    protected NavMeshAgent _agent;
    protected Rigidbody _rb;
    protected Animator _animator;

    [Header("General References")]
    [SerializeField] protected Transform _playerTransform;
    [SerializeField] protected LayerMask _groundLayer, _playerLayer;
    public bool IsEnemyActivated;

    [Header("General Attack Settings")]
    [SerializeField] protected GameObject AttackPrefab;
    [SerializeField] protected float _timeBetweenAttacks;

    [Header("Stun Settings")]
    public bool IsStunned;
    public ParticleSystem _stunEffect;

    [Header("Avoidance Settings")]
    [Tooltip("Ignores all other enemies with higher number. Lower value means higher imprortance.")]
    [SerializeField][Range(1, 50)] protected int _enemyAvoidancePriority;
    [Tooltip("Gives a random priority between a given Min & Max values.")]
    [SerializeField] protected bool _randomPriority;
    [SerializeField] protected int _MinRandomAvoidanceNumber;
    [SerializeField] protected int _MaxRandomAvoidanceNumber;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        EnemyAvoidanceInit();
    }

    protected virtual void Update()
    {
        if (_playerTransform != null)
        {
            PlayerDetaction();
            EnemyStateMachine();
        }

        _animator.SetFloat("Velocity", _agent.velocity.magnitude);
    }

    protected virtual void EnemyAvoidanceInit()
    {
        if (_randomPriority)
        {
            if (_MinRandomAvoidanceNumber <= 0 || _MaxRandomAvoidanceNumber <= _MinRandomAvoidanceNumber)
            {
                _MinRandomAvoidanceNumber = 1;
                _MaxRandomAvoidanceNumber = 50;
            }
            _enemyAvoidancePriority = Random.Range(_MinRandomAvoidanceNumber, _MaxRandomAvoidanceNumber);
        }

        if (_enemyAvoidancePriority <= 0)
            _enemyAvoidancePriority = Random.Range(1, 50);

        _agent.avoidancePriority = _enemyAvoidancePriority;
    }

    protected virtual void PlayerDetaction()
    {
    }

    protected virtual void EnemyStateMachine()
    {

        if (!IsEnemyActivated)
        {
            print(name + " is not activated.");
            _agent.SetDestination(transform.position); // Make sure enemy doesn't move
        }
        else
        {
            if (IsStunned && _stunEffect != null && !_stunEffect.isPlaying)
            {
                _stunEffect.Play();
                Stunned();
            }
            else if (!IsStunned && _stunEffect.isPlaying)
                _stunEffect.Stop();


            // Animator Test
            
        }
    }

    protected virtual void ChasePlayer()
    {
        if (_playerTransform != null)
            _agent.SetDestination(_playerTransform.position);
    }

    protected virtual void Stunned()
    {
        _rb.freezeRotation = true;
        _rb.Sleep();  // Freeze Position
        _agent.SetDestination(transform.position);
    }

    protected virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }

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
