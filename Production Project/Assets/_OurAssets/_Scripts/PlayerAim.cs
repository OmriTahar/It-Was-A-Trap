using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] Camera _cam;
    [SerializeField] GameObject _outlinePrefab, _trapPrefab, _wallPrefab, _line;
    [SerializeField] float maxDistance = 5f;

    GameObject currentActiveAttack;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool active = false;

    private void Awake()
    {
        currentActiveAttack = Instantiate(_outlinePrefab);

        ToggleDraw();
    }

    private void LateUpdate()
    {
        if (active)
            UpdateAim();
    }

    private void UpdateAim()
    {
        RaycastHit hit;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, groundMask))
        {
            currentActiveAttack.transform.position = (hit.point - transform.position).magnitude < maxDistance ? 
            hit.point : hit.point.normalized * maxDistance;

            //print($"hit ground: {hit.point}");
        }
    }

    public void ToggleDraw()
    {
        active = !active;
        _outlinePrefab.SetActive(active);
        //Line.SetActive(active);
    }

}