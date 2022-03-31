using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    private NavMeshAgent _agent;

    [Header("General Settings")]
    [SerializeField] int _health;
    [SerializeField] Transform _playerTransform;
    [SerializeField] LayerMask _groundLayer, _playerLayer;
    [SerializeField] Image _playerDetectionImage;

    [Header("Patroling")]
    [SerializeField] Vector3 _moveDestination;
    [SerializeField] bool _isDestinationSet;
    [SerializeField] float _DestinationPointRange;

    [Header("Chasing")]
    [SerializeField] float _playerSightRange;
    [SerializeField] bool _isPlayerInSight;

    [Header("Attacking")]
    [SerializeField] float _attackRange, _timeBetweenAttacks;
    [SerializeField] bool _isPlayerInAttackRange, _alreadyAttacked;
    public GameObject EnemyProjectile;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check for sight and attack range
        _isPlayerInSight = Physics.CheckSphere(transform.position, _playerSightRange, _playerLayer);
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayer);

        // Enemy States
        if (!_isPlayerInSight && !_isPlayerInAttackRange) Patroling();
        if (_isPlayerInSight && !_isPlayerInAttackRange) ChasePlayer();
        if (_isPlayerInAttackRange && _isPlayerInSight) AttackPlayer();
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
        if (_isPlayerInSight)
            _playerDetectionImage.gameObject.SetActive(true);
        else
            _playerDetectionImage.gameObject.SetActive(false);

        _agent.SetDestination(_playerTransform.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move while attacking
        _agent.SetDestination(transform.position);

        transform.LookAt(_playerTransform);

        if (!_alreadyAttacked)
        {
            /// Attack code here
            Rigidbody rb = Instantiate(EnemyProjectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);
    }
}
