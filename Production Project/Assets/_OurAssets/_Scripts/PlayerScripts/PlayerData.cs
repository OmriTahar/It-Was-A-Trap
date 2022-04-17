using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon { Trap, Wall}
public class PlayerData : Unit
{
    public static PlayerData Instance;
    Queue<GameObject> activeTraps = new Queue<GameObject>();
    Queue<GameObject> activeWalls = new Queue<GameObject>();
    [SerializeField] internal GameObject _trapPrefab, _wallPrefab;
    [SerializeField] int _wallAmmo, _trapAmmo, _maxTrapAmmo = 3, _maxWallAmmo = 3;
    [SerializeField] internal Weapon CurrentWeapon;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _wallAmmo = _maxWallAmmo;
        _trapAmmo = _maxTrapAmmo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeaponPrefab();
        }
    }

    void Attack()
    {
        switch (CurrentWeapon)
        {
            case Weapon.Trap:
                if (_trapAmmo <= 0)
                {
                    GameObject firstTrap = activeTraps.Dequeue();
                    Destroy(firstTrap);
                    if (_trapAmmo < _maxTrapAmmo)
                    {
                        _trapAmmo++;
                    }
                }

                GameObject trap = Instantiate(_trapPrefab, PlayerAim.Instance._outline.transform.position, Quaternion.identity);
                _trapAmmo--;
                activeTraps.Enqueue(trap);
                break;

            case Weapon.Wall:
                if (_wallAmmo <= 0)
                {
                    GameObject firstWall = activeWalls.Dequeue();
                    Destroy(firstWall);
                    if (_wallAmmo < _maxWallAmmo)
                    {
                        _wallAmmo++;
                    }
                }

                GameObject wall = Instantiate(_wallPrefab, PlayerAim.Instance._outline.transform.position, Quaternion.identity);
                _wallAmmo--;
                activeWalls.Enqueue(wall);
                break;
            default:
                break;
        }
    }

    void SwitchWeaponPrefab()
    {
        switch (CurrentWeapon)
        {
            case Weapon.Trap:
                CurrentWeapon = Weapon.Wall;
                Instance._trapPrefab.SetActive(false);
                Instance._wallPrefab.SetActive(true);
                break;
            case Weapon.Wall:
                CurrentWeapon = Weapon.Trap;
                Instance._wallPrefab.SetActive(false);
                Instance._trapPrefab.SetActive(true);
                break;
            default:
                break;
        }
    }

}