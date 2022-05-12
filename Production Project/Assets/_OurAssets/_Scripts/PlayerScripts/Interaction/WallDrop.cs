using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDrop : Interactable
{

    [SerializeField] GameObject _wallToDrop;

    private void Start()
    {
        _wallToDrop.SetActive(false);
    }

    public override void OnInteraction()
    {
        _wallToDrop.SetActive(true);
        Destroy(gameObject);
    }
}
