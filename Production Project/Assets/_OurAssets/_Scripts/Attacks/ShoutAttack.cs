using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutAttack : Attack
{

    [Header("Refrences")]
    public BoxCollider MyCollider;
    public Renderer MyRenderer;

    [Header("Settings")]
    [SerializeField] float _activationTime = 3f;
    [SerializeField] float _decayTime = 3f;

    [Header("Colors")]
    [SerializeField] Color _chargeColor = new Color(100, 0, 0);
    [SerializeField] Color _activationColor = new Color(255, 0, 0);

    private bool _alreadyAttacked;
    public bool ReadyToDetonate = false;


    private void Awake()
    {
        MyCollider = GetComponent<BoxCollider>();
        MyRenderer = GetComponent<Renderer>();

        MyRenderer.material.color = _chargeColor;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !_alreadyAttacked && ReadyToDetonate)
        {
            _alreadyAttacked = true;
            ReadyToDetonate = false;

            _attackedUnit = other.gameObject.GetComponent<Unit>();
            _attackedUnit.RecieveDamage(this);

            if (_causeStun)
            {
                StartCoroutine(StunPlayer(other));
                StartCoroutine(Decay(10f));
            }
            else
                StartCoroutine(Decay(10));

        }
        else
        {
            StartCoroutine(Decay(10));
        }
    }


    public void ActivateShout()
    {
        StartCoroutine(ShoutCoroutine());
    }

    private IEnumerator ShoutCoroutine()
    {
        print("Changing color!");
        MyRenderer.material.color = Color.Lerp(_chargeColor, _activationColor, _activationTime - Time.deltaTime);

        yield return new WaitForSeconds(_activationTime);
        print("Color Changed!");
        ReadyToDetonate = true;
    }

    private IEnumerator Decay(float decayTime)
    {
        yield return new WaitForSeconds(decayTime);

        print("Shout decayed");

        ReadyToDetonate = false;
        _alreadyAttacked = false;

        gameObject.SetActive(false);
        MyRenderer.material.color = _chargeColor;
    }

    //public void SetMe(ShoutsPool myPool) // Can also later be used to set Damage and other variables to the projectile
    //{
    //    _shoutsPool = myPool;
    //}
}
