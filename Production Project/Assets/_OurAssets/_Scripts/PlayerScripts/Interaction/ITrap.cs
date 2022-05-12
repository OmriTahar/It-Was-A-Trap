using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITrap : Attack
{

    [Header("Trap Settings")]
    public bool _isActivated = false;
    [SerializeField] bool _isTouchedGround = false;
    [SerializeField] float _dropSpeed = 5f;
    [SerializeField] float _waitBeforeKillBunny = 1f;

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


}
