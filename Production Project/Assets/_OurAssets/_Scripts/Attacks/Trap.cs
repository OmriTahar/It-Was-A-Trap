using System.Collections;
using UnityEngine;

public class Trap : Attack
{
    [Header("Settings")]
    [SerializeField] private float _placementDelay = 1f;
    [SerializeField] private float _decayTime = 5f;
    [SerializeField] private float _destroyAfterActivationTime = 2.5f;

    [Header("Particles")]
    [SerializeField] private GameObject _hitParticle;

    [Header("Trap Refrences")]
    [SerializeField] Animator _trapAnimator;
    [SerializeField] GameObject _actualTrapGameObject;

    [Header("Trap Settings")]
    public bool _isActivated = false;
    [SerializeField] float _waitBeforeKillBunny = 1f;
    [SerializeField] float _waitBeforeDestroyTrap = 3f;
    [SerializeField] float _animationDuration;

    private BaseEnemyAI _trappededEnemy;
    private Collider _myCollider;
    private bool _wasActivated = false;

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        TrapsPool.ActiveTrapsQueue.Enqueue(gameObject);
        StartCoroutine(PlaceTrap());
    }

    private void OnDisable()
    {
        if (_trappededEnemy)
            UnTrapEnemy(_trappededEnemy);

        TrapsPool.ReadyToFireTrapsQueue.Enqueue(gameObject);

        _hitParticle.SetActive(false);
        _myCollider.enabled = false;
        _wasActivated = false;

        _actualTrapGameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider other)
    {
        //checking if object isn't null as "TriggerEnter" can happen more then once, comparing to enemy tag and testing if it wasn't activated

        if (other != null && other.tag == "Enemy" && !_wasActivated)
        {
            _wasActivated = true;
            _trapAnimator.SetTrigger("CloseTrap");

            var enemyAI = other.GetComponent<BaseEnemyAI>();
            StartCoroutine(StunEnemy(enemyAI));
            _hitParticle.SetActive(true);

            StartCoroutine(DestroyAfterActivation());
        }
    }

    public IEnumerator PlaceTrap()
    {
        _myCollider.enabled = false;

        yield return new WaitForSeconds(_placementDelay);

        _actualTrapGameObject.SetActive(true);
        _myCollider.enabled = true;

        StartCoroutine(Decay());
    }

    public IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private IEnumerator DestroyAfterActivation()
    {
        yield return new WaitForSeconds(_destroyAfterActivationTime);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private IEnumerator StunEnemy(BaseEnemyAI enemyAI)
    {
        TrapEnemy(enemyAI);
        yield return new WaitForSeconds(_animationDuration);

        _trappededEnemy.RecieveDamage(this);

        yield return new WaitForSeconds(_stunDuration);

        UnTrapEnemy(enemyAI);
    }

    private void TrapEnemy(BaseEnemyAI enemyAI)
    {
        if (enemyAI)
        {
            _trappededEnemy = enemyAI;
            _trappededEnemy.transform.position = transform.position;
            _trappededEnemy.IsEnemyActivated = false;
            _trappededEnemy.IsStunned = true;
        }
    }

    private void UnTrapEnemy(BaseEnemyAI enemyAI)
    {
        if (_trappededEnemy)
        {

            _trappededEnemy.IsEnemyActivated = true;
            _trappededEnemy.IsStunned = false;
            _trappededEnemy = null;
            print("Im freeeeeee!");
        }
    }



}