using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BaseEnemyAI : Unit
{
    protected NavMeshAgent _agent;
    protected Rigidbody _rb;

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


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
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
        }
    }

    protected virtual void ChasePlayer()
    {
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
