using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeName { MovementSpeed, HP, DashCooldown, Range, MaxTraps, MaxWalls }
public class Upgrade : MonoBehaviour
{
    [SerializeField] internal UpgradeName myName;
    [SerializeField] string myDescription;
    [SerializeField] internal float bunniesToUnlock;
    [SerializeField] float changeBy;

    internal bool unlocked = false;

    private void OnEnable()
    {
        if (!unlocked && UpgradesList.UpgradeList != null)
            UpgradesList.UpgradeList.Add(this);
    }

    private void OnDisable()
    {
        if (unlocked && UpgradesList.UpgradeList != null)
            UpgradesList.UpgradeList.Remove(this);
    }

    private void ActivateUpgrade()
    {
        switch (myName)
        {
            case UpgradeName.MovementSpeed:
                break;
            case UpgradeName.HP:
                break;
            case UpgradeName.DashCooldown:
                break;
            case UpgradeName.Range:
                break;
            case UpgradeName.MaxTraps:
                break;
            case UpgradeName.MaxWalls:
                break;
            default:
                break;
        }
    }

    internal void Unlock()
    {
        if (!unlocked)
        {
            unlocked = true;
            ActivateUpgrade();
            enabled = false;
        }
    }

}