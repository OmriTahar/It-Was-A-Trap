using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum WeaponType { Trap, Wall }
public class PlayerData : Unit
{
    public static PlayerData Instance;
    internal bool _isAllowedToShoot = true;

    [Header("UI")]
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _winScreen;
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

    private PlayerController _myPlayerController;
    private SpriteRenderer _outlineRenderer;
    internal int bunnyCount = 0;
    private bool _loseCondition = false;
    private bool _winCondition = false;
    //for sharon to delete:
    private Color orange = new Color(1, 0.5f, 0);
    private Animator _animator;

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

        _animator = GetComponent<Animator>();
        _myPlayerController = GetComponent<PlayerController>();
    }

    //ondisable?
    protected override void Start()
    {
        base.Start();

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"{bunnyCount}";

        _outlineRenderer = PlayerAim.Instance.outline.transform.GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isAllowedToShoot)
        {
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.Q))
                SwitchWeaponPrefab();

            if (Input.GetKeyDown(KeyCode.Mouse0))
                if (canShoot && clearToShoot)
                    Attack();
        }

        if (_loseCondition && Input.anyKeyDown)
        {
            _gameOverScreen.SetActive(false);
            _loseCondition = false;
            SceneManager.LoadScene(1);
        }

        if (_winCondition && Input.anyKeyDown)
        {
            _winScreen.SetActive(false);
            _winCondition = false;
            SceneManager.LoadScene(0);
        }
    }

    public void OnWin()
    {
        _winCondition = true;

        _myPlayerController.IsAllowedToMove = false;
        _myPlayerController.IsAllowedToRotate = false;
        _isAllowedToShoot = false;

        _winScreen.SetActive(true);
    }

    protected override void OnDeath()
    {
        _loseCondition = true;
        _gameOverScreen.SetActive(true);
    }

    public void AddScore()
    {
        bunnyCount++;

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"{bunnyCount}";

        AbilityEligabilityCheck();
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

                FMODUnity.RuntimeManager.PlayOneShot("event:/Magic/Magic Trap Box");
                _animator.Play("TrapCast_Animation", 1);

                canShoot = false;
                Invoke("ResetAttack", _timeBetweenAttacks);
                break;

            case WeaponType.Wall:

                GameObject wall = CoverPool.GetCoverFromPool();
                Vector3 spawnPos = new Vector3(PlayerAim.Instance.outline.transform.position.x, PlayerAim.Instance.outline.transform.position.y + wall.transform.localScale.y / 2, PlayerAim.Instance.outline.transform.position.z);
                wall.transform.position = spawnPos;
                wall.transform.rotation = Quaternion.identity;

                Vector3 rotateWallTo = new Vector3(transform.position.x, wall.transform.position.y, transform.position.z);
                wall.transform.LookAt(rotateWallTo);

                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Card");
                _animator.Play("CardsCast_Animation", 1);

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
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color);
                break;
            case WeaponType.Wall:
                _outlineRenderer.sprite = _wallOutlineSprite;
                _currentAmmoAmountText.text = CoverPool.ReadyToFireCoversQueue.Count.ToString();
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color);
                break;
            default:
                _outlineRenderer.sprite = _trapOutlineSprite;
                _currentAmmoAmountText.text = TrapsPool.ReadyToFireTrapsQueue.Count.ToString();
                IsClearUIChange(currentWeapon, clearToShoot, _outlineRenderer.color);
                break;
        }
    }

    private void IsClearUIChange(WeaponType currWeapon, bool clearnShot, Color myColor)
    {
        if (clearnShot && myColor != Color.white)
            _outlineRenderer.color = Color.white;

        else if (!clearnShot && myColor != orange)
            _outlineRenderer.color = orange;

        else if (clearnShot && myColor != Color.white)
            _outlineRenderer.color = Color.white;

        else if (!clearnShot && myColor != orange)
            _outlineRenderer.color = orange;
    }

    private void ResetAttack()
    {
        canShoot = true;
    }

    private void AbilityEligabilityCheck()
    {
        Upgrade unlockableUpgrade = null;

        foreach (var item in UpgradesList.UpgradeList)
            if (item && bunnyCount >= item.bunniesToUnlock)
                unlockableUpgrade = item;

        if (unlockableUpgrade)
        {
            unlockableUpgrade.Unlock();
            ActivateUIUnlockedSkill(unlockableUpgrade);
        }
    }

    private void ActivateUIUnlockedSkill(Upgrade unlockedUpgrade)
    {
        print($"Congrats your {unlockedUpgrade.myName} has been upgraded");
    }

}