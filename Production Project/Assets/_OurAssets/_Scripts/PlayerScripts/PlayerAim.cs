using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public static PlayerAim Instance;
    [SerializeField] Camera _cam;
    [SerializeField] GameObject _outlinePrefab, _line, _trapOutline, _WallOutline;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] float _maxDistance = 5f;
    [SerializeField] bool _active = false;
    internal bool _canShoot = true;
    internal GameObject _outline;
    GameObject _currentAttackOutline;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _currentAttackOutline = _trapOutline;
        _outline = Instantiate(_outlinePrefab, transform);
        //_outline = Instantiate(_currentAttackOutline, transform);
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
            Vector3 distanceVector;
            if ((distanceVector = (hit.point - transform.position)).magnitude <= _maxDistance)
            {
                _outline.transform.position = hit.point;
            }
            else
            {
                distanceVector.y = -1;
                _outline.transform.position = transform.position + distanceVector.normalized * _maxDistance;
                //normalized prolly change the Y axis so it's not really flat
            }
        }
        else
        {
            _canShoot = false;
        }
    }

    public void ToggleDraw()
    {
        _active = !_active;
        _outlinePrefab.SetActive(_active);
        //_line.SetActive(active);
    }

}