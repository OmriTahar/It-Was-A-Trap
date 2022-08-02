using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrap : Attack
{

    [Header("Trap Refrences")]
    [SerializeField] Animator _trapAnimator;
    [SerializeField] BoxCollider _catchCollider;

    [Header("Trap Settings")]
    public bool _isActivated = false;
    [SerializeField] float _dropSpeed = 5f;
    [SerializeField] float _waitBeforeKillBunny = 1f;
    [SerializeField] float _waitBeforeDestroyTrap = 3f;

    private WaitForSeconds _waitBeforeKillBunnyCoroutine;
    private bool _isTouchedGround = false;


    private void Awake()
    {
        _waitBeforeKillBunnyCoroutine = new WaitForSeconds(_waitBeforeKillBunny);
    }

    void Update()
    {
        if (!_isTouchedGround &&_isActivated)
            transform.position += new Vector3(0, -1, 0) * _dropSpeed * Time.deltaTime;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            _attackedUnit = other.gameObject.GetComponent<Unit>();
            _attackedUnit.RecieveDamage(this, false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isTouchedGround = true;
            StartCoroutine(Decay());
        }
    }

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(_waitBeforeDestroyTrap);
        Destroy(gameObject);
    }
}
