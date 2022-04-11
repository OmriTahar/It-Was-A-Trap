using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject Line;
    [SerializeField] GameObject OutlinePrefab;
    [SerializeField] float AbilityRange = 3f;
    [SerializeField] float Distance = 5f;

    GameObject currentActiveAttack;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool active = false;

    private void Awake()
    {
        currentActiveAttack = Instantiate(OutlinePrefab);

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
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, groundMask))
        {
            currentActiveAttack.transform.position = (hit.point - transform.position).magnitude < Distance ? 
            hit.point : hit.point.normalized * Distance;

            print($"hit ground: {hit.point}");
        }
    }

    public void ToggleDraw()
    {
        active = !active;
        OutlinePrefab.SetActive(active);
        //Line.SetActive(active);
    }

}