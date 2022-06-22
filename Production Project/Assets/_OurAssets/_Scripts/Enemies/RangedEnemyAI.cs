using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class RangedEnemyAI : BaseEnemyAI
{

    private ProjectilePool _rangedProjectilePool;

    [Header("Ranged Attack Settings")]
    [SerializeField] Transform _rangedShootPoint;
    [SerializeField] float _rangedShootForce;

    [Header("Fleeing")]
    [SerializeField] float _startFleeFromPlayer_Range;
    [SerializeField] float _fleeingDuration;
    [SerializeField] float _fleeDistance = 10f;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _canFlee;
    [SerializeField] bool _isFleeing;

    private Vector3 _directionToPlayer;
    private WaitForSeconds _fleeDurationCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _rangedProjectilePool = GetComponent<ProjectilePool>();
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
                ChasePlayer();

            if (!_isFleeing)
            {
                if (_isPlayerTooClose && _canFlee)
                    StartCoroutine(Flee());

                if ((!_isPlayerTooClose && _isPlayerInAttackRange || _isPlayerTooClose && !_canFlee))
                    RangeAttack();
            }
        }
    }

    private void RangeAttack()
    {
        _agent.SetDestination(transform.position); // Make sure enemy doesn't move while attacking
        transform.LookAt(_playerTransform);

        if (!_isAlreadyAttacked)
        {
            GameObject carrot = _rangedProjectilePool.GetProjectileFromPool();
            carrot.transform.position = _rangedShootPoint.position;
            carrot.transform.rotation = Quaternion.identity;

            Rigidbody rb = carrot.GetComponent<Rigidbody>();
            rb.AddForce((_playerTransform.position - carrot.transform.position).normalized * _rangedShootForce, ForceMode.Impulse);

            Vector3 rotateCarrotTo = new Vector3(transform.position.x, carrot.transform.position.y, transform.position.z);
            carrot.transform.LookAt(rotateCarrotTo);

            _isAlreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private bool CanEnemyFlee()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * _fleeDistance;
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
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * _fleeDistance;
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
