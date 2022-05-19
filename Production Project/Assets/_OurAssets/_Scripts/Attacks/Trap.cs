using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Attack
{
    [Header("Refrences")]
    [SerializeField] GameObject _hitParticle;

    [Header("Settings")]
    [SerializeField] float _placementTime = 1f;                
    [SerializeField] float _decayTime = 6f;
    [SerializeField] float _destroyAfterActivationTime = 2.5f;  // Should always be longer than stun duration!
    [SerializeField] float _stunDuration = 2f;

    private Collider _myCollider;
    private bool _isPlaced = false;
    private bool _wasActivated = false;
    private Renderer _myRenderer;
    private Color _activatedColor = Color.blue;

    private void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        _myCollider = GetComponent<Collider>();

        _myCollider.enabled = false;
        _myRenderer.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(PlaceTrap());
    }

    private IEnumerator PlaceTrap()
    {
        yield return new WaitForSeconds(_placementTime);

        _isPlaced = true;
        _myCollider.enabled = true;
        _myRenderer.enabled = true;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (_isPlaced)
        {
            StartCoroutine(Decay());

            if (other.tag == "Enemy" && !_wasActivated)
            {
                _wasActivated = true;
                _myRenderer.material.color = _activatedColor;

                var enemyAI = other.GetComponent<EnemyAI>();
                enemyAI.RecieveDamage(this);
                StartCoroutine(StunEnemy(enemyAI));

                _hitParticle.SetActive(true);
                StartCoroutine(DestroyAfterActivation());
            }
        }
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);

        if (!_wasActivated)
            Destroy(gameObject);
    }

    private IEnumerator DestroyAfterActivation()
    {
        yield return new WaitForSeconds(_destroyAfterActivationTime);
        Destroy(gameObject);
    }

    private IEnumerator StunEnemy(EnemyAI enemyAI)
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

    private void OnDestroy()
    {
        PlayerData.Instance.currentTrapAmount++;
    }
}