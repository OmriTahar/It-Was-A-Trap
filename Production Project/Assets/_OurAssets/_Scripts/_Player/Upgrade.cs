using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeName { MovementSpeed, HP, DashCooldown, Range, MaxTraps, MaxWalls }
public abstract class Upgrade : MonoBehaviour
{
    [SerializeField] protected UpgradeName myName;
    [SerializeField] protected string myDescription;
    [SerializeField] protected float bunniesToUnlock;
    [SerializeField] protected float changeBy;

    internal bool unlocked = false;

    protected abstract void ActivateUpgrade();

    private void Unlock()
    {
        if (!unlocked && PlayerData.Instance.bunnyCount >= bunniesToUnlock)
        {
            unlocked = true;
            UpgradesList.instance.ActiveUpgrades.Add(myName, this);
            ActivateUpgrade();
        }
    }

}