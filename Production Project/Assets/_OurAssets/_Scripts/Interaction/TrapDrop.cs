using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDrop : Interactable
{

    [SerializeField] FallingTrap _trapToDrop;

    public override void OnInteraction()
    {
        _trapToDrop._isActivated = true;
        Destroy(gameObject);
    }
}
