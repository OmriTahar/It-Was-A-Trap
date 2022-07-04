using System.Collections;
using UnityEngine;

public class Trap : Attack
{
    #region Variables
    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _placementDelay = 1f;
    [SerializeField] private float _decayTime = 5f;
    [SerializeField] private float _destroyAfterActivationTime = 2.5f;

    [Header("Particles")]
    [SerializeField] private GameObject _hitParticle;

    [Header("Trap Refrences")]
    [SerializeField] private GameObject _graphicsGO;
    [SerializeField] private Animator _animator;

    private BaseEnemyAI _trappededEnemy;
    private Collider _myCollider;
    private bool _activated = false;
    #endregion

    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        //Adds to active traps queue
        TrapsPool.ActiveTrapsQueue.Enqueue(gameObject);

        //Basically Fire function
        StartCoroutine(PlaceTrap());
    }

    private void OnDisable()
    {
        //if trapped enemy not null, release it
        if (_trappededEnemy)
            UnTrapEnemy();

        //Throw back to ammo queue
        TrapsPool.ReadyToFireTrapsQueue.Enqueue(gameObject);

        //Deactivating Traps' related components
        _hitParticle.SetActive(false);
        _graphicsGO.SetActive(false);
        _myCollider.enabled = false;
        _activated = false;
    }

    public override void OnTriggerEnter(Collider other)
    {
        //checking if object isn't null as "TriggerEnter" can happen more then once, comparing to enemy tag and testing if trap not activated
        if (other != null && other.tag == "Enemy" && !_activated)
        {
            _activated = true;

            _animator.SetTrigger("CloseTrap");

            var enemyAI = other.GetComponent<BaseEnemyAI>();

            StartCoroutine(StunEnemy(enemyAI));

            StartCoroutine(DestroyAfterActivation());
        }
    }

    public IEnumerator PlaceTrap()
    {
        //Can be deleted but its here as a safe measure
        _myCollider.enabled = false;
        yield return new WaitForSeconds(_placementDelay);

        //Turning trap graphics and collider
        _graphicsGO.SetActive(true);
        _myCollider.enabled = true;       

        //Counting till death
        StartCoroutine(Decay());
    }

    public IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);

        //if trap already turned off before reaching this code
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private IEnumerator DestroyAfterActivation()
    {
        yield return new WaitForSeconds(_destroyAfterActivationTime);

        //if trap already turned off before reaching this code
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private IEnumerator StunEnemy(BaseEnemyAI enemyAI)
    {
        //Locking and shutting down AI
        TrapEnemy(enemyAI);

        //Waiting for Animation to finish
        yield return new WaitForSeconds(_animationDuration);

        //Damage Trapped AI, if not null
        if (_trappededEnemy)
            _trappededEnemy.RecieveDamage(this);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Magic/Magic Box Caught Enemy");

        //Actual stun, _stunDuration = stun you want - animation
        yield return new WaitForSeconds(_stunDuration - _animationDuration);

        //if enemy not dead, release from stun
        UnTrapEnemy();
    }

    private void TrapEnemy(BaseEnemyAI enemyAI)
    {
        //if not null (died before reaching this code), and if trap cause stun
        if (enemyAI && _causeStun)
        {
            //Cache enemy instance
            _trappededEnemy = enemyAI;

            //Moving enemy to middle of trap, with his Y
            _trappededEnemy.transform.position = new Vector3(transform.position.x, _trappededEnemy.transform.position.y, transform.position.z);

            //Activating Particles
            _hitParticle.SetActive(true);

            //Deactivating enemy
            _trappededEnemy.IsEnemyActivated = false;
            _trappededEnemy.IsStunned = true;

            print($"Enemy: {_trappededEnemy.name} was trapped");
        }
    }

    private void UnTrapEnemy()
    {
        //if cached enemy not null
        if (_trappededEnemy)
        {
            //Reactivating enemy
            _trappededEnemy.IsEnemyActivated = true;
            _trappededEnemy.IsStunned = false;

            //Releasing enemy from cache
            _trappededEnemy = null;

            print($"Enemy: {_trappededEnemy.name} is now free");
        }
    }

}