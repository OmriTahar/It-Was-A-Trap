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
    [SerializeField][ReadOnlyInspector] WeaponType _currentWeapon;
    [SerializeField][ReadOnlyInspector] bool canShoot = true;
    [SerializeField] float _timeBetweenAttacks;

    [Header("UI")]
    public TextMeshProUGUI CurrentAmmoAmount_Text;
    public Image CurrentWeapon_ImageSlot;
    [SerializeField] Sprite _coverImage;
    [SerializeField] Sprite _trapImage;

    //weapon pools
    private Queue<GameObject> _activeWallsQueue = new Queue<GameObject>();
    private TrapsPool _trapsPool;
    private WallsPool _wallsPool;

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

        if ((_trapsPool = gameObject.GetComponent<TrapsPool>()) == null)
            Debug.Log("TrapPool Script is missing from Player");
        if ((_wallsPool = gameObject.GetComponent<WallsPool>()) == null)
            Debug.Log("WallPool Script is missing from Player");
    }

    private void Update()
    {
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeaponPrefab();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canShoot)
                Attack();
        }
    }

    void Attack()
    {
        switch (_currentWeapon)
        {
            case WeaponType.Trap:

                if (PlayerAim.Instance.clearToShoot && canShoot)
                {
                    GameObject trap = _trapsPool.GetProjectileFromPool();
                    trap.transform.position = PlayerAim.Instance.outline.transform.position;
                    trap.transform.rotation = Quaternion.identity;

                    canShoot = false;
                    Invoke(nameof(ResetAttack), _timeBetweenAttacks);
                }
                break;

            case WeaponType.Wall:

                if (PlayerAim.Instance.clearToShoot && canShoot)
                {
                    if (_wallsPool.WallQueue.Count <= 0)
                    {
                        GameObject firstWall = _activeWallsQueue.Dequeue();
                        firstWall.GetComponent<Wall>().ReturnMySelfToPool(firstWall);
                    }

                    GameObject wall = _wallsPool.GetProjectileFromPool();
                    wall.transform.position = PlayerAim.Instance.outline.transform.position;
                    wall.transform.rotation = Quaternion.identity;

                    Vector3 rotateWallTo = new Vector3(transform.position.x, wall.transform.position.y, transform.position.z);
                    wall.transform.LookAt(rotateWallTo);

                    _activeWallsQueue.Enqueue(wall);

                    canShoot = false;
                    Invoke(nameof(ResetAttack), _timeBetweenAttacks);
                }
                break;
            default:
                break;
        }
    }

    private void SwitchWeaponPrefab()
    {
        switch (_currentWeapon)
        {
            case WeaponType.Trap:
                _currentWeapon = WeaponType.Wall;
                break;
            case WeaponType.Wall:
                _currentWeapon = WeaponType.Trap;
                break;
            default:
                break;
        }
    }

    private void UpdateUI()
    {
        switch (_currentWeapon)
        {
            case WeaponType.Trap:
                CurrentWeapon_ImageSlot.sprite = _trapImage;
                CurrentAmmoAmount_Text.text = _trapsPool.TrapPoolQueue.Count.ToString();
                break;
            case WeaponType.Wall:
                CurrentWeapon_ImageSlot.sprite = _coverImage;
                CurrentAmmoAmount_Text.text = _wallsPool.WallQueue.Count.ToString();
                break;
            default:
                CurrentWeapon_ImageSlot.sprite = _trapImage;
                CurrentAmmoAmount_Text.text = _trapsPool.TrapPoolQueue.Count.ToString();
                break;
        }
    }

    private void ResetAttack()
    {
        canShoot = true;
    }
}
