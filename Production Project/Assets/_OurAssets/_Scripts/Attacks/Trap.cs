using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Attack
{

    [Header("Refrences")]
    [SerializeField] GameObject _hitParticle;
    [SerializeField] TrapsPool _trapPool;

    [Header("Settings")]
    [SerializeField] float _placementTime = 1f;                
    [SerializeField] float _decayTime = 6f;
    [SerializeField] float _destroyAfterActivationTime = 2.5f;  // Should always be longer than stun duration!

    [Header("Stun Settings")]
    [SerializeField] float _stunDuration = 2f;

    private Collider _myCollider;
    private Renderer _myRenderer;
    private bool _isPlaced = false;
    private bool _wasActivated = false;
    private Color _placedColor = Color.red;
    private Color _activatedColor = Color.blue;


    private void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        _myCollider = GetComponent<Collider>();

        _myCollider.enabled = false;
        _myRenderer.enabled = false;
    }

    private void OnEnable()
    {
        StartCoroutine(PlaceTrap());
    }

    public IEnumerator PlaceTrap()
    {
        yield return new WaitForSeconds(_placementTime);

        _isPlaced = true;
        _myCollider.enabled = true;
        _myRenderer.enabled = true;

        StartCoroutine(NewDecay());
    }


    public override void OnTriggerEnter(Collider other)
    {
        if (_isPlaced)
        {
            if (other.tag == "Enemy" && !_wasActivated)
            {
                _wasActivated = true;
                _myRenderer.material.color = _activatedColor;

                var enemyAI = other.GetComponent<BaseEnemyAI>();
                enemyAI.RecieveDamage(this);


                StartCoroutine(StunEnemy(enemyAI));

                _hitParticle.SetActive(true);
                StartCoroutine(DestroyAfterActivation());
            }
        }
    }

    public IEnumerator NewDecay()
    {
        print("Decay started.");
        yield return new WaitForSeconds(_decayTime);
        _trapPool.ReturnTrapToPool(gameObject);
    }

    private void OnDisable()
    {
        _isPlaced = false;
        _myCollider.enabled = false;
        _myRenderer.enabled = false;

        _hitParticle.SetActive(false);
        _myRenderer.material.color = _placedColor;
    }

    private IEnumerator DestroyAfterActivation()
    {
        yield return new WaitForSeconds(_destroyAfterActivationTime);
        _trapPool.ReturnTrapToPool(gameObject);
    }

    public void SetMe(TrapsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    {
        _trapPool = myPool;
    }

    private IEnumerator StunEnemy(BaseEnemyAI enemyAI)
    {
        enemyAI.IsEnemyActivated = false;
        enemyAI.IsStunned = true;

        print("Im stunned!");

        if (enemyAI.gameObject.activeSelf && enemyAI.gameObject != null)
        {
            yield return new WaitForSeconds(_stunDuration);

            enemyAI.IsEnemyActivated = true;
            enemyAI.IsStunned = false;
            print("Im freeeeeee!");
        }
    }

    //private IEnumerator Decay()
    //{
    //    yield return new WaitForSeconds(_decayTime);

    //    if (!_wasActivated)
    //        Destroy(gameObject);
    //}

    //private void OnDestroy()
    //{
    //    PlayerData.Instance.currentTrapAmount++;
    //}
}