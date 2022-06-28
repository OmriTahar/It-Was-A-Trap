using System.Collections;
using UnityEngine;

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
    [SerializeField] LayerMask _playerShotsLayer;

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

            //_isLeapPathBlocked = Physics.Raycast(transform.position, transform.forward, _maxLeapDistance, _playerShotsLayer);

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, _maxLeapDistance);

            if (hit.collider != null && hit.collider.tag != "Player")
            {
                _isLeapPathBlocked = true;
                print("Something else is in front of me!");
                return;
            }
            else if (hit.collider != null && hit.collider.tag == "Player")
            {
                _isLeapPathBlocked = false;
                print("Player is in front of me!");
                return;
            }
            else
            {
                _isLeapPathBlocked = false;
            }
        }
    }

    protected override void EnemyStateMachine()
    {
        base.EnemyStateMachine();

        if (IsEnemyActivated)
        {
            transform.LookAt(_playerTransform);

            if (!_hasLeaped)
            {
                if (!_isPlayerInAttackRange || _isLeapPathBlocked)
                {
                    _isMovingBackwards = false;
                    ChasePlayer();
                }
                else if (_isPlayerInAttackRange && _isPlayerTooClose)
                {
                    CreateLeapRange();
                }

                if (_isPlayerInAttackRange && !_isPlayerTooClose && !_isLeapPathBlocked)
                {
                    StartCoroutine(Leap());
                }
            }
        }
    }

    private void CreateLeapRange()
    {
        _directionToPlayer = (transform.position - _playerTransform.position).normalized * _minLeapDistance;
        _newDestination = transform.position + _directionToPlayer;
        _agent.SetDestination(_newDestination);
    }

    private IEnumerator Leap()
    {
        _hasLeaped = true;
        _agent.SetDestination(transform.position);

        yield return _waitBeforeLeapCoroutine;

        if (_isLeapPathBlocked)
            print("Path is NOT clear");
        else
            print("Path IS clear");

        if ((_playerTransform.position - transform.position).magnitude <= _maxLeapDistance && !_isLeapPathBlocked)
        {
            _animator.SetTrigger("Leap");

            yield return _startLeapLogicCorutine;
            if (!IsStunned && _playerTransform != null)
            {
                AttackPrefab.SetActive(true);
                _rb.AddForce((_playerTransform.position - transform.position) * _leapPower, ForceMode.Impulse);
                _isMovingBackwards = true;
            }

            yield return _waitAfterLeapCoroutine;
            if (!IsStunned && _playerTransform != null)
            {
                AttackPrefab.SetActive(false);
                _hasLeaped = false;
            }
        }
        else // Leap distance is too long
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
