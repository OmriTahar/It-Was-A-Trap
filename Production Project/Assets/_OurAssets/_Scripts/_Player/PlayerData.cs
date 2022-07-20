using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum WeaponType { Trap, Wall }
public class PlayerData : Unit
{

    #region Variables

    #region Exposed Variables

    public static PlayerData Instance;
    internal bool _isAllowedToShoot = true;
    internal int bunnyCount = 0;

    #endregion

    #region UI References

    [Header("References")]
    public Animator HitEffectAnimator;

    [Header("UI References")]
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private TextMeshProUGUI _currentAmmoAmountText;
    [SerializeField] private TextMeshProUGUI _currentBunnyCountText;
    [SerializeField] private Image _currentWeaponImage;
    [SerializeField] private Sprite _coverImage;
    [SerializeField] private Sprite _trapImage;
    [SerializeField] private Sprite _trapOutlineSprite;
    [SerializeField] private Sprite _wallOutlineSprite;

    #endregion

    #region Settings

    [Header("Weapon Settings")]
    [SerializeField][ReadOnlyInspector] internal WeaponType currentWeapon;
    [SerializeField][ReadOnlyInspector] internal bool canShoot = true, clearToShoot = true;
    [SerializeField] private float _timeBetweenAttacks;

    [Header("Win / Death Screen Delay Settings")]
    [SerializeField] bool _isCanPressContinue = false;
    [SerializeField] float _delayBeforeAllowingToPressContinue = 3f;

    #endregion

    #region Private 

    private PlayerController _myPlayerController;
    private SpriteRenderer _outlineRenderer;
    private bool _loseCondition = false;
    private bool _winCondition = false;
    private bool _mute = false;
    private Color _red = new Color(1, 0, 0);

    private Animator _animator;
    public Animator PlayerAnimatorGetter => _animator;
    private int _trapCastAnimationHash;
    private int _cardsCastAnimationHash;

    #endregion

    #endregion

    #region Methods

    #region Unity Callbacks

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

    private void OnEnable()
    {
        OnPlayerKilled += SetLoseCondition;
    }

    private void OnDisable()
    {
        OnPlayerKilled -= SetLoseCondition;
    }

    protected override void Start()
    {
        base.Start();

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"{bunnyCount}";

        _outlineRenderer = PlayerAim.Instance.outline.transform.GetComponentInChildren<SpriteRenderer>();

        AnimationHashInit();
        OutcomeScreensInit();
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

        if (Input.GetKey(KeyCode.M))
        {
            _mute = !_mute;
            FMODUnity.RuntimeManager.PauseAllEvents(_mute);
        }

        if (_winCondition || _loseCondition)
            GameIsOver();
    }

    #endregion

    #region Win/Lose Conditions

    private void OutcomeScreensInit()
    {
        _gameOverScreen.SetActive(false);
        _loseCondition = false;

        _winScreen.SetActive(false);
        _winCondition = false;
    }

    public void OnWin()
    {
        _winCondition = true;

        _myPlayerController.IsAllowedToMove = false;
        _myPlayerController.IsAllowedToRotate = false;
        _isAllowedToShoot = false;

        _winScreen.SetActive(true);
        Invoke("ToggleCanPressContinue", _delayBeforeAllowingToPressContinue);
    }

    private void SetLoseCondition() 
    {
        _loseCondition = true;
        _gameOverScreen.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/Player/Magician Death");
        Invoke("ToggleCanPressContinue", _delayBeforeAllowingToPressContinue);
    }

    private void ToggleCanPressContinue()
    {
        _isCanPressContinue = true;
    }

    private void GameIsOver()
    {
        if (_loseCondition && _isCanPressContinue && Input.anyKeyDown)
            SceneManager.LoadScene(1);

        if (_winCondition && _isCanPressContinue && Input.anyKeyDown)
            SceneManager.LoadScene(0);
    }

    #endregion

    #region Attack Methods

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

                FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/Magic/Magic Trap Box");
                _animator.Play(_trapCastAnimationHash, 1);

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

                FMODUnity.RuntimeManager.PlayOneShot("event:/Sound/Player/Card");
                _animator.Play(_cardsCastAnimationHash, 1);

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

    private void IsClearUIChange(WeaponType currWeapon, bool clearnShot, Color myColor)
    {
        if (clearnShot && myColor != Color.white)
            _outlineRenderer.color = Color.white;

        else if (!clearnShot && myColor != _red)
            _outlineRenderer.color = _red;

        else if (clearnShot && myColor != Color.white)
            _outlineRenderer.color = Color.white;

        else if (!clearnShot && myColor != _red)
            _outlineRenderer.color = _red;
    }

    private void ResetAttack()
    {
        canShoot = true;
    }

    #endregion

    #region UI Methods

    public void AddScore()
    {
        bunnyCount++;

        if (_currentBunnyCountText)
            _currentBunnyCountText.text = $"{bunnyCount}";

        AbilityEligabilityCheck();
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

    #endregion

    #region Upgrade System

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

    #endregion

    #region Extras

    private void AnimationHashInit()
    {
        _trapCastAnimationHash = Animator.StringToHash("TrapCast_Animation");
        _cardsCastAnimationHash = Animator.StringToHash("CardsCast_Animation");
    }

    #endregion

    #endregion

}