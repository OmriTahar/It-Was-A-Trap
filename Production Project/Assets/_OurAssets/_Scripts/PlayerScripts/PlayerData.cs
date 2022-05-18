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
    public GameObject trapPrefab, wallPrefab;
    public int maxTrapAmmo = 3, maxWallAmmo = 3, currentCoverAmount, currentTrapAmount;
    public float timeBetweenShots;
    public Weapon currentWeapon;

    [Header("UI")]
    public TextMeshProUGUI CurrentAmmoUI;
    public Image CurrentWeaponUI;
    [SerializeField] Sprite _coverImage;
    [SerializeField] Sprite _trapImage;

    internal Quaternion shotRotation = Quaternion.identity;
    private bool canShoot = true;
    internal int bunnyCount;

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
            if (canShoot)
            {
                Attack();
                UpdateUI();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            shotRotation = new Quaternion(0, shotRotation.y + 15, 0, 0);
            print(shotRotation);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            shotRotation = new Quaternion(0, shotRotation.y - 15, 0, 0);
            print(shotRotation);
        }

    }

    void Attack()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                if (PlayerAim.Instance._clearToShoot)
                {
                    if (activeTraps.Count == maxTrapAmmo)
                    {
                        GameObject firstTrap = activeTraps.Dequeue();
                        Destroy(firstTrap);
                    }

                    GameObject trap = Instantiate(trapPrefab, PlayerAim.Instance._outline.transform.position, shotRotation);
                    activeTraps.Enqueue(trap);
                    currentTrapAmount--;
                }
                break;

            case Weapon.Wall:
                if (PlayerAim.Instance._clearToShoot)
                {
                    if (activeWalls.Count == maxWallAmmo)
                    {
                        GameObject firstWall = activeWalls.Dequeue();
                        Destroy(firstWall);
                    }

                    GameObject wall = Instantiate(wallPrefab, PlayerAim.Instance._outline.transform.position, shotRotation);
                    activeWalls.Enqueue(wall);
                    currentCoverAmount--;
                }
                break;
            default:
                break;
        }

        StartCoroutine(WaitToShoot());
    }

    private void SwitchWeaponPrefab()
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

        shotRotation = Quaternion.identity;
    }

    private void UpdateUI()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                CurrentWeaponUI.sprite = _trapImage;
                CurrentAmmoUI.text = currentTrapAmount.ToString();
                break;
            case Weapon.Wall:
                CurrentWeaponUI.sprite = _coverImage;
                CurrentAmmoUI.text = currentCoverAmount.ToString();
                break;
            default:
                CurrentWeaponUI.sprite = _trapImage;
                CurrentAmmoUI.text = currentTrapAmount.ToString();
                break;
        }
    }

    IEnumerator WaitToShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

}