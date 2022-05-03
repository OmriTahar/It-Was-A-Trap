using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] internal UpgradeName myUpgrade;
    bool collected = false;

    private void OnEnable()
    {
        if (collected)
        {
            Destroy(gameObject);
            return;
        }
        UpgradesList.instance.AddUpgrade(this);
    }

    private void OnDisable()
    {
        UpgradesList.instance.RemoveUpgrade(this);
    }

}