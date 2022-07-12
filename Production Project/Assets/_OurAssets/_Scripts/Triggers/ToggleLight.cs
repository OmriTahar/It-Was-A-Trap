using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLight : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Light _myLight;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] CapsuleCollider _capsuleCollider;

    [Header("Settings")]
    [Tooltip("If true: Light is allways ON")]
    [SerializeField] bool _isAlwaysEnabled = true;

    private bool _isPlayerInRange;

    private void Awake()
    {
        if (!_isAlwaysEnabled)
        {
            _myLight.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isAlwaysEnabled)
        {
            _myLight.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !_isAlwaysEnabled)
        {
            _myLight.enabled = false;
        }
    }

}
