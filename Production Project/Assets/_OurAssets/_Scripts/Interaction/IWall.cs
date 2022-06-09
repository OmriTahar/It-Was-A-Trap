using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IWall : Attack
{

    [Header("Wall Settings")]
    public bool _isActivated = false;
    [SerializeField] float _dropSpeed = 5f;

    bool _isTouchedGround = false;
    bool _hasAttacked = false;


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
        }
        else if (!_isTouchedGround && collision.gameObject.tag == "Player" && !_hasAttacked) // Ensures wall only hit player once
        {
            _hasAttacked = true;
            collision.gameObject.GetComponent<Unit>().RecieveDamage(this);
        }
    }
}
