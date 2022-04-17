using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon { Trap, Block}
public class PlayerData : Unit
{
    public static PlayerData Instance;
    [SerializeField] internal Weapon CurrentWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

}