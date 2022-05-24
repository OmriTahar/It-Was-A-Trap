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
    [SerializeField] GameObject _outlinePrefab;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask groundMask;

    [Header("Aim Settings")]
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float minDistance = 0.5f;
    [SerializeField] float outLineHight = -1f;
    [SerializeField] bool _canAim = false;

    [Header("Interaction")]
    [SerializeField] LayerMask _interactableLayers;
    [SerializeField] bool _isAllowedToInteract;
    [SerializeField] bool _canInteract;
    [SerializeField] float _interactionDistance = 22f;
    [SerializeField] GameObject _canInteractText;
    private Interactable _currentInteractable;

    internal bool clearToShoot = true;
    internal GameObject outline;

    //for gizmos can delete later
    Vector3 gizmoSize = new Vector3(2.5f, 0.5f, 2.5f);
    Vector3 gizmoPos;

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

        outline = Instantiate(_outlinePrefab, transform);

        ToggleDraw();
    }

    private void Update()
    {
        if (_canAim)
        {
            UpdateAim();

            if (Input.GetKeyDown(KeyCode.F) && _canInteract)
                Interact();

        }
    }

    //for gizmos can delete later
    private void OnDrawGizmos()
    {
        if (clearToShoot)
            Gizmos.color = new Color(0, 1, 0, .2f);
        else
            Gizmos.color = new Color(1, 0, 0, .2f);

        Gizmos.DrawCube(gizmoPos, gizmoSize);
    }

    private void UpdateAim()
    {
        RaycastHit _hit;
        Ray _ray = _cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _hit, 1000, groundMask))
        {
            clearToShoot = !Physics.CheckBox(_hit.point, new Vector3(1.25f, .25f, 1.25f), Quaternion.identity, obstacleMask);

            Vector3 distanceVector;

            if ((distanceVector = (_hit.point - transform.position)).magnitude < minDistance || (_hit.point - transform.position).magnitude < 0)
            {
                distanceVector.y = -1;
                outline.transform.position = transform.position + distanceVector.normalized * minDistance;
                outline.transform.position = new Vector3(outline.transform.position.x, _hit.point.y, outline.transform.position.z);
            }
            else if ((distanceVector = (_hit.point - transform.position)).magnitude > maxDistance)
            {
                distanceVector.y = -1;
                outline.transform.position = transform.position + distanceVector.normalized * maxDistance;
                outline.transform.position = new Vector3(outline.transform.position.x, _hit.point.y, outline.transform.position.z);
            }
            else
            {
                outline.transform.position = _hit.point;
            }

            //for gizmos can delete later
            gizmoPos = outline.transform.position;
        }

        InteractionCheck(_hit, _ray);
    }

    public void ToggleDraw()
    {
        _canAim = !_canAim;
        _outlinePrefab.SetActive(_canAim);
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