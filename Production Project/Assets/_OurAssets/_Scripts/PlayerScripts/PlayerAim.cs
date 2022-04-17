using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public static PlayerAim Instance;
    [SerializeField] Camera _cam;
    [SerializeField] GameObject _outlinePrefab, _line;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] float _maxDistance = 5f;
    [SerializeField] bool _active = false;

    internal GameObject _outline;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _outline = Instantiate(_outlinePrefab, transform);
        //remove this after we decide when we want aim to start V
        ToggleDraw();
    }

    private void Update()
    {
        if (_active)
            UpdateAim();
    }

    private void UpdateAim()
    {
        RaycastHit hit;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, _groundMask))
        {
            _outline.transform.position = (hit.point - transform.position).magnitude <= _maxDistance ? 
            hit.point : /*even if i normalize and multiply it doesnt seem work*/ hit.point;
            //print($"delta norm:{(hit.point - transform.position).normalized}");
        }
    }

    public void ToggleDraw()
    {
        _active = !_active;
        _outlinePrefab.SetActive(_active);
        //_line.SetActive(active);
        //switch (PlayerData.Instance.CurrentWeapon)
        //{
        //    case Weapon.Trap:
        //        PlayerData.Instance._wallPrefab.SetActive(!active);
        //        PlayerData.Instance._trapPrefab.SetActive(active);
        //        break;
        //    case Weapon.Wall:
        //        PlayerData.Instance._trapPrefab.SetActive(!active);
        //        PlayerData.Instance._wallPrefab.SetActive(active);
        //        break;
        //    default:
        //        break;
        //}
    }

}