using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Weapon { Trap, Wall }
public class PlayerData : Unit
{

    public static PlayerData Instance;

    [Header("Weapon Settings")]
    internal Queue<GameObject> activeTraps = new Queue<GameObject>();
    internal Queue<GameObject> activeWalls = new Queue<GameObject>();
    public int maxTrapAmmo = 3, maxWallAmmo = 3, currentCoverAmount, currentTrapAmount, bunnyCount;
    public GameObject trapPrefab, wallPrefab;
    public Weapon currentWeapon;

    [Header("UI")]
    public TextMeshProUGUI CurrentAmmoUI;
    public Image CurrentWeaponImage;
    [SerializeField] Sprite _coverImage;
    [SerializeField] Sprite _trapImage;


    private void Awake()
    {
        #region Singelton

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        #endregion

        currentCoverAmount = maxWallAmmo;
        currentTrapAmount = maxTrapAmmo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeaponPrefab();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
            UpdateUI();
        }
    }

    void Attack()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:

                if (activeTraps.Count == maxTrapAmmo)
                {
                    GameObject firstTrap = activeTraps.Dequeue();
                    Destroy(firstTrap);
                }

                if (PlayerAim.Instance._canShoot)
                {
                    GameObject trap = Instantiate(trapPrefab, PlayerAim.Instance._outline.transform.position, Quaternion.identity);
                    activeTraps.Enqueue(trap);
                    currentTrapAmount--;
                }

                break;

            case Weapon.Wall:

                if (activeWalls.Count == maxWallAmmo)
                {
                    GameObject firstWall = activeWalls.Dequeue();
                    Destroy(firstWall);
                }

                if (PlayerAim.Instance._canShoot)
                {
                    GameObject wall = Instantiate(wallPrefab, PlayerAim.Instance._outline.transform.position, Quaternion.identity);
                    activeWalls.Enqueue(wall);
                    currentCoverAmount--;
                }

                break;
            default:
                break;
        }
    }

    void SwitchWeaponPrefab()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                currentWeapon = Weapon.Wall;
                break;
            case Weapon.Wall:
                currentWeapon = Weapon.Trap;
                break;
            default:
                break;
        }
    }

    public void UpdateUI()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                CurrentWeaponImage.sprite = _trapImage;
                CurrentAmmoUI.text = currentTrapAmount.ToString();
                break;
            case Weapon.Wall:
                CurrentWeaponImage.sprite = _coverImage;
                CurrentAmmoUI.text = currentCoverAmount.ToString();
                break;
            default:
                CurrentWeaponImage.sprite = _trapImage;
                CurrentAmmoUI.text = currentTrapAmount.ToString();
                break;
        }
    }
}