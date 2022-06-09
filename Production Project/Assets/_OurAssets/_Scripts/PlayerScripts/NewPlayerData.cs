using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum WeaponType { Trap, Wall }

public class NewPlayerData : MonoBehaviour
{
    public static NewPlayerData Instance;

    [Header("Weapon Settings")]
    [SerializeField] WeaponType _currentWeapon;
    [SerializeField] bool _isAlreadyAttacked;
    [SerializeField] float _timeBetweenAttacks;

    [Header("Weapon Pools")]
    [SerializeField] TrapsPool _trapsPool;
    [SerializeField] WallsPool _wallsPool;
    private Queue<GameObject> _activeWallsQueue = new Queue<GameObject>();

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
    }

    private void Update()
    {
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeaponPrefab();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canShoot)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        switch (_currentWeapon)
        {
            case WeaponType.Trap:

                if (PlayerAim.Instance.clearToShoot && !_isAlreadyAttacked)
                {
                    GameObject trap = _trapsPool.GetProjectileFromPool();
                    trap.transform.position = PlayerAim.Instance.outline.transform.position;
                    trap.transform.rotation = Quaternion.identity;

                    _isAlreadyAttacked = true;
                    Invoke(nameof(ResetAttack), _timeBetweenAttacks);
                }
                break;

            case WeaponType.Wall:

                if (PlayerAim.Instance.clearToShoot && !_isAlreadyAttacked)
                {
                    if (_wallsPool.WallPoolQueue.Count <= 0)
                    {
                        GameObject firstWall = _activeWallsQueue.Dequeue();
                        firstWall.GetComponent<Wall>().ReturnToPool();
                    }

                    GameObject wall = _wallsPool.GetProjectileFromPool();
                    wall.transform.position = PlayerAim.Instance.outline.transform.position;
                    wall.transform.rotation = Quaternion.identity;

                    Vector3 rotateWallTo = new Vector3(transform.position.x, wall.transform.position.y, transform.position.z);
                    wall.transform.LookAt(rotateWallTo);

                    _activeWallsQueue.Enqueue(wall);

                    _isAlreadyAttacked = true;
                    Invoke(nameof(ResetAttack), _timeBetweenAttacks);
                }
                break;
            default:
                break;
        }

        //StartCoroutine(WaitToShoot());
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
                CurrentAmmoAmount_Text.text = _wallsPool.WallPoolQueue.Count.ToString();
                break;
            default:
                CurrentWeapon_ImageSlot.sprite = _trapImage;
                CurrentAmmoAmount_Text.text = _trapsPool.TrapPoolQueue.Count.ToString();
                break;
        }
    }

    private void ResetAttack()
    {
        _isAlreadyAttacked = false;
    }

    //IEnumerator WaitToShoot()
    //{
    //    canShoot = false;
    //    yield return new WaitForSeconds(timeBetweenShots);
    //    canShoot = true;
    //}

}
