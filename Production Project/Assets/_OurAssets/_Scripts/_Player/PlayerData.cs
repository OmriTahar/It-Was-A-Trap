using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum WeaponType { Trap, Wall }
public class PlayerData : Unit
{
    public static PlayerData Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _currentAmmoAmountText;
    [SerializeField] private TextMeshProUGUI _currentBunnyCountText;
    [SerializeField] private Image _currentWeaponImage;
    [SerializeField] private Sprite _coverImage;
    [SerializeField] private Sprite _trapImage;
    [SerializeField] private Sprite _trapOutlineSprite;
    [SerializeField] private Sprite _wallOutlineSprite;

    [Header("Weapon Settings")]
    [SerializeField][ReadOnlyInspector] internal WeaponType currentWeapon;
    [SerializeField][ReadOnlyInspector] internal bool canShoot = true, clearToShoot = true;
    [SerializeField] private float _timeBetweenAttacks;

    private SpriteRenderer _outlineRenderer;
    internal int bunnyCount = 0;

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
    }

    protected override void Start()
    {
        base.Start();

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"X{bunnyCount}";

        _outlineRenderer = PlayerAim.Instance.outline.transform.GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeaponPrefab();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (canShoot && clearToShoot)
                Attack();
    }

    public void AddScore()
    {
        bunnyCount++;

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"X{bunnyCount}";

        //if (UpgradesList.UpgradeList.Count > 0)
            //AbilityEligabilityCheck();
    }

    private void Attack()
    {
        switch (currentWeapon)
        {
            case WeaponType.Trap:

                GameObject trap = TrapsPool.GetTrapFromPool();
                trap.transform.position = PlayerAim.Instance.outline.transform.position;
                trap.transform.rotation = Quaternion.identity;

                canShoot = false;
                Invoke("ResetAttack", _timeBetweenAttacks);
                break;

            case WeaponType.Wall:

                GameObject wall = WallsPool.GetWallFromPool();
                Vector3 spawnPos = new Vector3(PlayerAim.Instance.outline.transform.position.x, PlayerAim.Instance.outline.transform.position.y + wall.transform.localScale.y / 2, PlayerAim.Instance.outline.transform.position.z);
                wall.transform.position = spawnPos;
                wall.transform.rotation = Quaternion.identity;

                Vector3 rotateWallTo = new Vector3(transform.position.x, wall.transform.position.y, transform.position.z);
                wall.transform.LookAt(rotateWallTo);

                canShoot = false;
                Invoke("ResetAttack", _timeBetweenAttacks);
                break;
            default:
                break;
        }
    }

    private void SwitchWeaponPrefab()
    {
        switch (currentWeapon)
        {
            case WeaponType.Trap:
                currentWeapon = WeaponType.Wall;
                break;
            case WeaponType.Wall:
                currentWeapon = WeaponType.Trap;
                break;
            default:
                break;
        }
    }

    private void UpdateUI()
    {
        switch (currentWeapon)
        {
            case WeaponType.Trap:
                _outlineRenderer.sprite = _trapOutlineSprite;

                _currentWeaponImage.sprite = _trapImage;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                break;
            case WeaponType.Wall:
                _outlineRenderer.sprite = _wallOutlineSprite;

                _currentWeaponImage.sprite = _coverImage;
                _currentAmmoAmountText.text = WallsPool.ReadyToFireWallsQueue.Count.ToString();
                break;
            default:
                _outlineRenderer.sprite = _trapOutlineSprite;

                _currentWeaponImage.sprite = _trapImage;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                break;
        }
    }

    private void ResetAttack()
    {
        canShoot = true;
    }

    private void AbilityEligabilityCheck()
    {
        foreach (var item in UpgradesList.UpgradeList)
        {
            print($"{item.Key}");
            if (item.Key == item.Value.myName)
            {
                print("Key and Value match");
                if (bunnyCount >= item.Value.bunniesToUnlock && !item.Value.unlocked)
                {
                    print("Unlocking ability");
                    item.Value.Unlock();
                    ActivateUIUnlockedSkill();
                }
            }
        }
    }

    private void ActivateUIUnlockedSkill()
    {
        print("UI celebrating unlock");
    }

}