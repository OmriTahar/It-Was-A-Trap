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
    public TextMeshProUGUI CurrentAmmoAmount_Text;
    public Image CurrentWeapon_ImageSlot;
    [SerializeField] Sprite _coverImage;
    [SerializeField] Sprite _trapImage;

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

    }

    void Attack()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                if (PlayerAim.Instance.clearToShoot)
                {
                    if (activeTraps.Count == maxTrapAmmo)
                    {
                        GameObject firstTrap = activeTraps.Dequeue();
                        Destroy(firstTrap);
                    }

                    GameObject trap = Instantiate(trapPrefab, PlayerAim.Instance.outline.transform.position, Quaternion.identity);
                    activeTraps.Enqueue(trap);
                    currentTrapAmount--;
                }
                break;

            case Weapon.Wall:
                if (PlayerAim.Instance.clearToShoot)
                {
                    if (activeWalls.Count == maxWallAmmo)
                    {
                        GameObject firstWall = activeWalls.Dequeue();
                        Destroy(firstWall);
                    }

                    GameObject wall = Instantiate(wallPrefab, PlayerAim.Instance.outline.transform.position, Quaternion.identity);

                    Vector3 rotateWallTo = new Vector3(transform.position.x, wall.transform.position.y, transform.position.z);
                    wall.transform.LookAt(rotateWallTo);
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
    }

    private void UpdateUI()
    {
        switch (currentWeapon)
        {
            case Weapon.Trap:
                CurrentWeapon_ImageSlot.sprite = _trapImage;
                CurrentAmmoAmount_Text.text = currentTrapAmount.ToString();
                break;
            case Weapon.Wall:
                CurrentWeapon_ImageSlot.sprite = _coverImage;
                CurrentAmmoAmount_Text.text = currentCoverAmount.ToString();
                break;
            default:
                CurrentWeapon_ImageSlot.sprite = _trapImage;
                CurrentAmmoAmount_Text.text = currentTrapAmount.ToString();
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