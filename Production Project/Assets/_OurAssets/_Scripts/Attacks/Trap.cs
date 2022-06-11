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

    private BaseEnemyAI _trappededEnemy;
    private Collider _myCollider;
    private Renderer _myRenderer;
    private Color _placedColor = Color.red;
    private Color _activatedColor = Color.blue;

    private void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        _myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        TrapsPool.ActiveTrapsQueue.Enqueue(gameObject);
        _myRenderer.material.color = _placedColor;

        StartCoroutine(PlaceTrap());
    }

    private void OnDisable()
    {
        if (_trappededEnemy)
            UnTrapEnemy(_trappededEnemy);

        TrapsPool.ReadyToFireTrapsQueue.Enqueue(gameObject);

        _hitParticle.SetActive(false);
        _myCollider.enabled = false;
        _myRenderer.enabled = false;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _myRenderer.material.color = _activatedColor;

            var enemyAI = other.GetComponent<BaseEnemyAI>();
            enemyAI.RecieveDamage(this);
            StartCoroutine(StunEnemy(enemyAI));

            _hitParticle.SetActive(true);

            StartCoroutine(DestroyAfterActivation());
        }
    }

    public IEnumerator PlaceTrap()
    {
        _myCollider.enabled = false;
        _myRenderer.enabled = false;

        yield return new WaitForSeconds(_placementDelay);

        _myCollider.enabled = true;
        _myRenderer.enabled = true;

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

        yield return new WaitForSeconds(_stunDuration);

        UnTrapEnemy(enemyAI);
    }

    private void TrapEnemy(BaseEnemyAI enemyAI)
    {
        //IsEnemyActivated should be removed and IsStunned = true should change him to IsEnemyActivated = false and vise verse
        if (enemyAI)
        {
            _trappededEnemy = enemyAI;
            _trappededEnemy.IsEnemyActivated = false;
            _trappededEnemy.IsStunned = true;
            print("Im stunned!");
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