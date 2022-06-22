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
    //we will need the two below don't touch <3 (sharon)
    //[SerializeField] private Sprite _trapOutlineOffSprite;
    //[SerializeField] private Sprite _wallOutlineOffSprite;

    [Header("Weapon Settings")]
    [SerializeField][ReadOnlyInspector] internal WeaponType currentWeapon;
    [SerializeField][ReadOnlyInspector] internal bool canShoot = true, clearToShoot = true;
    [SerializeField] private float _timeBetweenAttacks;

    private SpriteRenderer _outlineRenderer;
    internal int bunnyCount = 0;

    //for sharon to delete:
    private Color orange = new Color(1, 0.5f, 0);

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

                Vector3 rotateTrapTo = new Vector3(transform.position.x, trap.transform.position.y, transform.position.z);
                trap.transform.LookAt(rotateTrapTo);

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
                _currentWeaponImage.sprite = _coverImage;
                break;
            case WeaponType.Wall:
                currentWeapon = WeaponType.Trap;
                _currentWeaponImage.sprite = _trapImage;
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
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color/*_outlineRenderer.sprite*/);
                break;
            case WeaponType.Wall:
                _outlineRenderer.sprite = _wallOutlineSprite;
                _currentAmmoAmountText.text = WallsPool.ReadyToFireWallsQueue.Count.ToString();
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color/*_outlineRenderer.sprite*/);
                break;
            default:
                _outlineRenderer.sprite = _trapOutlineSprite;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color/*_outlineRenderer.sprite*/);
                break;
        }
    }

    private void IsClearUIChange(WeaponType currWeapon, bool clearnShot, Color myColor/*Sprite currentSprite*/)
    {
        if (clearnShot && myColor != Color.white/*currWeapon == WeaponType.Trap && currentSprite != _trapOutlineSprite*/)
            _outlineRenderer.color = Color.white/*currentSprite = _trapOutlineSprite*/;

        else if (!clearnShot && myColor != orange/*currWeapon == WeaponType.Trap && currentSprite != _trapOutlineOffSprite*/)
            _outlineRenderer.color = orange/*currentSprite = _trapOutlineOffSprite*/;

        else if (clearnShot && myColor != Color.white/*currWeapon == WeaponType.Wall && currentSprite != _wallOutlineSprite*/)
            _outlineRenderer.color = Color.white/*currentSprite = _wallOutlineSprite*/;

        else if (!clearnShot && myColor != orange/*currWeapon == WeaponType.Wall && currentSprite != _wallOutlineOffSprite*/)
            _outlineRenderer.color = orange/*currentSprite = _wallOutlineOffSprite*/;
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