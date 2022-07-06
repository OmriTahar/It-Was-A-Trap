using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LeaperAI : BaseEnemyAI
{

    [Header("Leap Settings")]
    [SerializeField] float _leapPower;
    [SerializeField] float _waitBeforeLeap;
    [SerializeField] float _waitAfterLeap;
    [SerializeField] float _playerIsTooCloseRange;
    [Tooltip("Cancles the leap if the player flee beyond this range.")]
    [SerializeField] float _maxLeapDistance;
    [SerializeField] float _minLeapDistance = 3f;
    [Tooltip("Player layer must be included even though he is not a leap path obstacle!")]
    [SerializeField] LayerMask _leapPathObstacleLayers;

    private Vector3 _directionToPlayer;
    private Vector3 _newDestination;

    [Header("Enemy Status")]
    [SerializeField] bool _isPlayerInAttackRange;
    [SerializeField] bool _isPlayerTooClose;
    [SerializeField] bool _isLeapPathBlocked;
    [SerializeField] bool _hasLeaped = false;
    [SerializeField] bool _isMovingBackwards;

    private WaitForSeconds _startLeapLogicCorutine = new WaitForSeconds(0.3f);
    private WaitForSeconds _waitBeforeLeapCoroutine;
    private WaitForSeconds _waitAfterLeapCoroutine;
    private bool _isLeapRayHitSomthing;
    private bool _canCreateLeapRange;


    protected override void Awake()
    {
        base.Awake();

        _waitBeforeLeapCoroutine = new WaitForSeconds(_waitBeforeLeap);
        _waitAfterLeapCoroutine = new WaitForSeconds(_waitAfterLeap);
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

            if (_isPlayerInAttackRange && _isLeapPathBlocked)
            {
                ChasePlayer();
            }

            if (!_hasLeaped)
            {
                if (!_isPlayerInAttackRange || _isLeapPathBlocked)
                {
                    _isMovingBackwards = false;
                    ChasePlayer();
                }
                else if (_isPlayerTooClose && _canCreateLeapRange)
                {
                    CreateLeapRange();
                }

                if (((_isPlayerInAttackRange && !_isPlayerTooClose) || (_isPlayerTooClose && !_canCreateLeapRange)) && !_isLeapPathBlocked)
                {
                    StartCoroutine(Leap());
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

        _hasLeaped = true;
        _agent.SetDestination(transform.position);

        if (_isLeapPathBlocked)
        {
            _hasLeaped = false;
            yield break;
        }

        yield return _waitBeforeLeapCoroutine;

        if (_isLeapPathBlocked)
        {
            _hasLeaped = false;
            yield break;
        }

        if ((_playerTransform.position - transform.position).magnitude <= _maxLeapDistance && !_isLeapPathBlocked)
        {
            _animator.SetTrigger("Leap");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Bunny/Bunny Start Leap Attack");

            yield return _startLeapLogicCorutine;

            if (!IsStunned && _playerTransform != null && !_isLeapPathBlocked)
            {              
                AttackPrefab.SetActive(true);
                _rb.AddForce((_playerTransform.position - transform.position) * _leapPower, ForceMode.Impulse);
                _isMovingBackwards = true;               
            }

            yield return _waitAfterLeapCoroutine;
            if (!IsStunned && _playerTransform != null && !_isLeapPathBlocked)
            {
                AttackPrefab.SetActive(false);
            }

            _hasLeaped = false;
        }
        else // Leap distance is too long or path is blocked
        {
            _hasLeaped = false;
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
