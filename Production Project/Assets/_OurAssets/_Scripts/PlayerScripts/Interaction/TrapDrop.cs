using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDrop : Interactable
{

    [SerializeField] ITrap _trapToDrop;

    private void Start()
    {
        _trapToDrop._isActivated = false;
    }

    public override void OnInteraction()
    {
        _trapToDrop._isActivated = true;
        Destroy(gameObject);
    }
}
