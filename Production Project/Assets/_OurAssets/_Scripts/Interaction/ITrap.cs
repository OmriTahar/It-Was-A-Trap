using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITrap : Attack
{

    [Header("Trap Refrences")]
    [SerializeField] Animator _trapAnimator;
    [SerializeField] BoxCollider _catchCollider;

    [Header("Trap Settings")]
    public bool _isActivated = false;
    [SerializeField] float _waitBeforeKillBunny = 1f;
    //[SerializeField] float _waitBeforeDestroyTrap = 3f;

    private bool _stopUpdate;
    private WaitForSeconds _waitBeforeKillBunnyCoroutine;


    private void Awake()
    {
        _catchCollider.enabled = false;
        _waitBeforeKillBunnyCoroutine = new WaitForSeconds(_waitBeforeKillBunny);
    }

    void Update()
    {
        if (_isActivated && !_stopUpdate)
        {
            _trapAnimator.SetTrigger("CloseTrap");
            StartCoroutine(KillBunny());
            _stopUpdate = true;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            _attackedUnit = other.gameObject.GetComponent<Unit>();
            _attackedUnit.RecieveDamage(this);

            if (_causeStun)
            {
                StartCoroutine(StunPlayer(other, _attackedUnit));
            }
        }
    }

    IEnumerator KillBunny()
    {
        yield return new WaitForSeconds(_waitBeforeKillBunny);
        _catchCollider.enabled = true;
    }

    //IEnumerator Decay()
    //{
    //    yield return new WaitForSeconds(_waitBeforeDestroyTrap);
    //    Destroy(gameObject);
    //}
}
