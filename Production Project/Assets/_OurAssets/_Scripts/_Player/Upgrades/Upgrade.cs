using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeName { MovementSpeed, HP, DashCooldown, Range, MaxTraps, MaxWalls }
public abstract class Upgrade : MonoBehaviour
{
    [SerializeField] internal UpgradeName myName;
    [SerializeField] protected string myDescription;
    [SerializeField] internal float bunniesToUnlock;
    [SerializeField] protected float changeBy;

    internal bool unlocked = false;

    private void OnEnable()
    {
        if (!unlocked && UpgradesList.UpgradeList != null)
            UpgradesList.UpgradeList.Add(myName, this);
    }

    private void OnDisable()
    {
        if (unlocked && UpgradesList.UpgradeList != null)
            UpgradesList.UpgradeList.Remove(myName);
    }

    protected abstract void ActivateUpgrade();

    internal void Unlock()
    {
        if (!unlocked)
        {
            this.unlocked = true;
            ActivateUpgrade();
            this.enabled = false;
        }
    }

}