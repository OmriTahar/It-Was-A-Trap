using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    private NavMeshAgent _agent;
    private Rigidbody _rb;

    [Header("General Settings")]
    [SerializeField] int _health;
    [SerializeField] Transform _playerTransform;
    [SerializeField] LayerMask _groundLayer, _playerLayer;

    [Header("Enemy TYPE")]
    public bool IsRangedEnemy;
    public bool IsLeapEnemy;

    [Header("Patroling")]
    [SerializeField] Vector3 _moveDestination;
    [SerializeField] bool _isDestinationSet;
    [SerializeField] float _DestinationPointRange;

    [Header("Chasing")]
    [SerializeField] float _playerSightRange;
    [SerializeField] bool _isPlayerInSight;
    public bool IsEnemyActivated;

    [Header("Attacking")]
    [SerializeField] Transform ShootPoint;
    [SerializeField] float ShootForce;
    [SerializeField] float _attackRange, _timeBetweenAttacks;
    [SerializeField] bool _isPlayerInAttackRange, _alreadyAttacked;
    public GameObject EnemyProjectile;

    [Header("Fleeing")]
    [SerializeField] float _moveBackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    private Vector3 _directionToPlayer;

    #region Jumping (Currently Not Used)

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
        //_isPlayerInSight = Physics.CheckSphere(transform.position, _playerSightRange, _playerLayer);

        if (IsEnemyActivated)
        {
            _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayer);
            _isPlayerTooClose = Physics.CheckSphere(transform.position, _moveBackRange, _playerLayer);

            if (_isPlayerTooClose)
            {
                _canFlee = CanEnemyFlee();
            }
        }
    }

    private void EnemyStateMachine()
    {
        if (!IsEnemyActivated)
        {
            print("Patroling");
            Patroling();
        }

        if (IsEnemyActivated && !_isPlayerInAttackRange)
        {
            print("Chasing");
            ChasePlayer();
        }

        if (IsRangedEnemy)
        {
            if (IsEnemyActivated && _isPlayerTooClose && _canFlee)
            {
                print("fleeing");
                Flee();
            }

            if (IsEnemyActivated && (!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
            {
                print("Attacking");
                RangeAttack();
            }
        }
        else if (IsLeapEnemy)
        {
            print("leaping");
            Leap();
        }
    }

    private void Leap()
    {

    }

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
        float randomZ = Random.Range(-_DestinationPointRange, _DestinationPointRange);
        float randomX = Random.Range(-_DestinationPointRange, _DestinationPointRange);

        _moveDestination = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(_moveDestination, -transform.up, 2f, _groundLayer))
            _isDestinationSet = true;
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(_playerTransform.position);
    }

    private void RangeAttack()
    {
        // Make sure enemy doesn't move while attacking
        _agent.SetDestination(transform.position);

        transform.LookAt(_playerTransform);

        if (!_alreadyAttacked)
        {
            /// Attack code here
            Rigidbody rb = Instantiate(EnemyProjectile, ShootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(ShootPoint.forward * ShootForce, ForceMode.Impulse);
            rb.AddForce(ShootPoint.up * (ShootForce / 2), ForceMode.Impulse);

            _alreadyAttacked = true;
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

    private void Flee()
    {
        _directionToPlayer = transform.position - _playerTransform.position;
        Vector3 newPosition = transform.position + _directionToPlayer;
        _agent.SetDestination(newPosition);
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    public void TakeDamage(int damage) // PLACE HOLDER
    {
        _health -= damage;

        if (_health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
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
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
