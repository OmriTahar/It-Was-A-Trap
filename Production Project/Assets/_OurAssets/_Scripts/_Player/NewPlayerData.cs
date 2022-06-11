using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum WeaponType { Trap, Wall }
public class NewPlayerData : Unit
{
    public static NewPlayerData Instance;

    [Header("Weapon Settings")]
    [SerializeField][ReadOnlyInspector] internal WeaponType currentWeapon;
    [SerializeField][ReadOnlyInspector] internal bool canShoot = true, clearToShoot = true;
    [SerializeField] private float _timeBetweenAttacks;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _currentAmmoAmountText;
    [SerializeField] private Image _currentWeaponImage;
    [SerializeField] private Sprite _coverImage;
    [SerializeField] private Sprite _trapImage;

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

    private void Attack()
    {
        switch (currentWeapon)
        {
            case WeaponType.Trap:

                GameObject trap = TrapsPool.GetProjectileFromPool();
                trap.transform.position = PlayerAim.Instance.outline.transform.position;
                trap.transform.rotation = Quaternion.identity;

                canShoot = false;
                Invoke("ResetAttack", _timeBetweenAttacks);
                break;

            case WeaponType.Wall:

                GameObject wall = WallsPool.GetWallFromPool();
                wall.transform.position = PlayerAim.Instance.outline.transform.position;
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
                _currentWeaponImage.sprite = _trapImage;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                break;
            case WeaponType.Wall:
                _currentWeaponImage.sprite = _coverImage;
                _currentAmmoAmountText.text = WallsPool.ReadyToFireWallsQueue.Count.ToString();
                break;
            default:
                _currentWeaponImage.sprite = _trapImage;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                break;
        }
    }

    private void ResetAttack()
    {
        canShoot = true;
    }
}
