using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverInteract : Interactable
{

    [SerializeField] GameObject _coverToDrop;

    private void Start()
    {
        _coverToDrop.SetActive(false);
    }

    public override void OnInteraction()
    {
        _coverToDrop.SetActive(true);
        Destroy(gameObject);
    }
}
