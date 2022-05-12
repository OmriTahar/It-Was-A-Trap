using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAim : MonoBehaviour
{

    public static PlayerAim Instance;

    [Header("Aim Refrences")]
    [SerializeField] Camera _cam;
    [SerializeField] GameObject _outlinePrefab, _line, _trapOutline, _WallOutline;
    [SerializeField] LayerMask _groundMask;

    [Header("Aim Settings")]
    [SerializeField] float _maxDistance = 5f;
    [SerializeField] bool _active = false;

    [Header("Interaction")]
    [SerializeField] LayerMask _interactableLayers;
    [SerializeField] bool _isAllowedToInteract;
    [SerializeField] bool _canInteract;
    [SerializeField] float _interactionDistance = 22f;
    [SerializeField] GameObject _canInteractText;
    private Interactable _currentInteractable;

    internal bool _canShoot = true;
    internal GameObject _outline;
    GameObject _currentAttackOutline;

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

        _canInteractText.SetActive(false);

        _currentAttackOutline = _trapOutline;
        _outline = Instantiate(_outlinePrefab, transform);
        //_outline = Instantiate(_currentAttackOutline, transform); -> remove this after we decide when we want aim to start V

        ToggleDraw();
    }

    private void Update()
    {
        if (_active)
        {
            UpdateAim();

            if (Input.GetKeyDown(KeyCode.F) && _canInteract)
            {
                Interact();
            }
        }
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
                _outline.transform.position = new Vector3(_outline.transform.position.x, hit.point.y, _outline.transform.position.z);
            }

            //Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
        else
        {
            _canShoot = false;
        }

        InteractionCheck(hit, ray);
    }

    public void ToggleDraw()
    {
        _active = !_active;
        _outlinePrefab.SetActive(_active);
        //_line.SetActive(active);
    }

    private void InteractionCheck(RaycastHit hit, Ray ray)
    {
        if (_isAllowedToInteract)
        {
            if (Physics.Raycast(ray, out hit, _interactionDistance, _interactableLayers))
            {
                _canInteract = true;
                _currentInteractable = hit.collider.GetComponent<Interactable>();
                _canInteractText.SetActive(true);
            }
            else
            {
                _canInteract = false;
                _currentInteractable = null;
                _canInteractText.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        if (_currentInteractable)
        {
            _currentInteractable.OnInteraction();
        }
    }
}