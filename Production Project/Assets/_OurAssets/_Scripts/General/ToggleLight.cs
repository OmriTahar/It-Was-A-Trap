using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLight : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Light _myLight;
    [SerializeField] LayerMask _playerLayer;

    [Header("Settings")]
    [Tooltip("If true: Light is allways ON")]
    [SerializeField] bool _isAlwaysEnabled = true;
    [SerializeField] float _enabledRange = 10f;

    private bool _isPlayerInRange;

    private void FixedUpdate()
    {
        if (_isAlwaysEnabled) return;

        _isPlayerInRange = Physics.CheckSphere(transform.position, _enabledRange, _playerLayer);

        if (_isPlayerInRange)
            _myLight.enabled = true;
        else
            _myLight.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _enabledRange);
    }
}
