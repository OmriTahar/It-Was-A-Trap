using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IWall : Attack
{

    [Header("Wall Settings")]
    [SerializeField] bool _isActivated = false;
    [SerializeField] bool _isTouchedGround = false;
    [SerializeField] float _dropSpeed = 5f;
    bool _hasAttacked = false;


    private void OnEnable()
    {
        _isActivated = true;
    }

    void Update()
    {
        if (_isActivated && !_isTouchedGround)
            transform.position += new Vector3(0, -1, 0) * _dropSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isTouchedGround = true;
            print("Touched Ground!");
        }
        else if (!_isTouchedGround && collision.gameObject.tag == "Player" && !_hasAttacked)
        {
            _hasAttacked = true;
            collision.gameObject.GetComponent<Unit>().RecieveDamage(this);
            print("Wall Fall Damage!");
        }
    }
}
