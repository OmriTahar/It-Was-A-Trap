using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Attack
{
    [SerializeField] GameObject _hitParticle;
    [SerializeField] float _decayTime;
    [SerializeField] float _stunDuration = 2f;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            var enemyAI = other.GetComponent<EnemyAI>();

            enemyAI.RecieveDamage(this);
            StartCoroutine(StunEnemy(enemyAI));

            _hitParticle.SetActive(true);
            StartCoroutine(Decay());
        }
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_decayTime);

        Destroy(gameObject);
    }

    private IEnumerator StunEnemy(EnemyAI enemyAI)
    {
        enemyAI.IsEnemyActivated = false;
        print("Im stunned!");

        yield return new WaitForSeconds(_stunDuration);
        enemyAI.IsEnemyActivated = true;
        print("Im freeeeeee!");

    }

    private void OnDestroy()
    {
        PlayerData.Instance._currentTrapAmount++;
    }

}