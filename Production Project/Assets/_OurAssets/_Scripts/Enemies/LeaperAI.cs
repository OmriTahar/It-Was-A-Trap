using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LeaperAI : BaseEnemyAI
{

    [Header("Designers Leap Settings")]
    [SerializeField] float _leapPower;
    [SerializeField] float _minLeapDistance = 3f;
    [Tooltip("Cancles the leap if the player flee beyond this range.")]
    [SerializeField] float _maxLeapDistance;
    [SerializeField] float _playerIsTooCloseRange;

    [Header("Debug Settings")]
    [Tooltip("Player layer must be included even though he is not a leap path obstacle!")]
    [SerializeField] LayerMask _leapPathObstacleLayers;
    [SerializeField] float _chargeAnimationDelay;
    [SerializeField] float _afterLeapDelay;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _isLeapPathBlocked;
    [SerializeField] bool _isLeaping = false;
    [SerializeField] bool _isMovingBackwards;

    private Vector3 _directionToPlayer;
    private Vector3 _newDestination;
    private bool _canCreateLeapRange;

    private WaitForSeconds _leapAnimationDelay = new WaitForSeconds(0.3f);
    private WaitForSeconds _chargeAnimationDelayCoroutine;
    private WaitForSeconds _afterLeapDelayCoroutine;


    protected override void Awake()
    {
        base.Awake();

        _chargeAnimationDelayCoroutine = new WaitForSeconds(_chargeAnimationDelay);
        _afterLeapDelayCoroutine = new WaitForSeconds(_afterLeapDelay);
    }

    protected override void Update()
    {
        base.Update();
        _animator.SetBool("isMovingBackwards", _isMovingBackwards);
    }

    protected override void PlayerDetaction()
    {
        if (IsEnemyActivated)
        {
            _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _unitAttackRange, _playerLayer);
            _isPlayerTooClose = Physics.CheckSphere(transform.position, _playerIsTooCloseRange, _playerLayer);

            if (_isPlayerTooClose)
                _canCreateLeapRange = CanCreateLeapRange();

            CheckLeapPath();
        }
    }

    protected override void EnemyStateMachine()
    {
        base.EnemyStateMachine();

        if (IsEnemyActivated)
        {
            transform.LookAt(_playerTransform);

            if (!_isLeaping)
            {
                if (IsCreatingAttackPath)
                {
                    if (HasReachedDestination())
                    {
                        IsCreatingAttackPath = false;
                    }
                }
                else
                {
                    if (((_isPlayerInAttackRange && !_isPlayerTooClose) || (_isPlayerTooClose && !_canCreateLeapRange)) && !_isLeapPathBlocked)
                    {
                        StartCoroutine(Leap());
                    }
                    else if (_isPlayerTooClose && _canCreateLeapRange)
                    {
                        CreateLeapRange();
                    }
                    else if (!_isPlayerInAttackRange || (_isPlayerInAttackRange && _isLeapPathBlocked))
                    {
                        ChasePlayer();
                    }
                }
            }
        }
    }

    private void CheckLeapPath()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, _maxLeapDistance, _leapPathObstacleLayers);

        if (hit.collider != null && hit.collider.tag != "Player")
        {
            _isLeapPathBlocked = true;
            return;
        }
        else if (hit.collider != null && hit.collider.tag == "Player")
        {
            _isLeapPathBlocked = false;
            return;
        }
        else
        {
            _isLeapPathBlocked = false;
        }
    }

    private void CreateLeapRange()
    {
        _myCurrentState = State.creatingRange;

        _directionToPlayer = (transform.position - _playerTransform.position).normalized * _minLeapDistance;
        _newDestination = transform.position + _directionToPlayer;
        _agent.SetDestination(_newDestination);
    }

    private bool CanCreateLeapRange()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * _minLeapDistance;
        _newDestination = transform.position + _directionToPlayer;

        if (_agent.CalculatePath(_newDestination, new NavMeshPath()))
        {
            return true;
        }
        else
            return false;
    }

    private IEnumerator Leap()
    {
        _myCurrentState = State.attacking;
        _isLeaping = true;

        _agent.SetDestination(transform.position);
        _rb.constraints = RigidbodyConstraints.FreezePosition; // Makes sure he is not being pushed
        _animator.SetTrigger("ChargeLeap");

        yield return _chargeAnimationDelayCoroutine;

        if ((_playerTransform.position - transform.position).magnitude <= _maxLeapDistance && !_isLeapPathBlocked)
        {
            _rb.constraints = RigidbodyConstraints.None;
            _animator.SetTrigger("Leap");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/Bunny/Bunny Start Leap Attack");

            yield return _leapAnimationDelay;

            AttackPrefab.SetActive(true);
            _rb.AddForce((_playerTransform.position - transform.position) * _leapPower, ForceMode.Impulse);
            _isMovingBackwards = true;

            yield return _afterLeapDelayCoroutine;

            AttackPrefab.SetActive(false);
            _isLeaping = false;
            _isMovingBackwards = false;
        }
        else // Leap distance is too long or path is blocked
        {
            _isLeaping = false;
            _animator.SetTrigger("CancleLeap");
            _rb.constraints = RigidbodyConstraints.None;
            CreateClearAttackPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerIsTooCloseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _unitAttackRange);

        Gizmos.color = Color.yellow;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * _maxLeapDistance;
        Gizmos.DrawRay(transform.position, direction);
    }

}
