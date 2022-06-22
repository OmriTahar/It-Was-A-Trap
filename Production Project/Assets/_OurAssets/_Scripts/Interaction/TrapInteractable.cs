using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInteractable : Interactable
{

    [SerializeField] ITrap _trapToActivate;

    private void Start()
    {
        _trapToActivate._isActivated = false;
    }

    public override void OnInteraction()
    {
        _trapToActivate._isActivated = true;
        Destroy(gameObject);
    }
}
