using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesList : MonoBehaviour
{
    public static UpgradesList instance;

    internal Dictionary<UpgradeName, Upgrade> ActiveUpgrades;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


}