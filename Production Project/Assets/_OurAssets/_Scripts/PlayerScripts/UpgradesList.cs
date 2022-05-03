using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeName { }
public class UpgradesList : MonoBehaviour
{
    public static UpgradesList instance;

    internal Dictionary<UpgradeName, GameObject> Upgrades;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        Upgrades.Add(upgrade.myUpgrade, upgrade.gameObject);
    }

    public void RemoveUpgrade(Upgrade upgrade)
    {
        Upgrades.Remove(upgrade.myUpgrade);
    }

}