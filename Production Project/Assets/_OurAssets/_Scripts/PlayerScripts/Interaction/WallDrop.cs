using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDrop : Interactable
{

    [SerializeField] IWall _wallToDrop;

    private void Start()
    {
        _wallToDrop._isActivated = false;
    }

    public override void OnInteraction()
    {
        _wallToDrop._isActivated = true;
        Destroy(gameObject);
    }
}
