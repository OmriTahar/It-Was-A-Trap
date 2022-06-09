using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITrap : Attack
{

    [Header("Trap Settings")]
    public bool _isActivated = false;
    [SerializeField] float _dropSpeed = 5f;
    [SerializeField] float _waitBeforeKillBunny = 1f;
    [SerializeField] float _waitBeforeDestroyTrap = 3f;

    private bool _isTouchedGround = false;


    void Update()
    {
        if (_isActivated && !_isTouchedGround)
            transform.position += new Vector3(0, -1, 0) * _dropSpeed * Time.deltaTime;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _isTouchedGround = true;
            print("Touched Ground!");
            StartCoroutine(Decay());
        }

        if (other.gameObject.tag == "Enemy")
        {
            StartCoroutine(KillBunny(other.gameObject));
        }
    }

    IEnumerator KillBunny(GameObject bunny)
    {
        yield return new WaitForSeconds(_waitBeforeKillBunny);
        Destroy(bunny);
        Destroy(gameObject);
    }

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(_waitBeforeDestroyTrap);
        Destroy(gameObject);
    }
}
