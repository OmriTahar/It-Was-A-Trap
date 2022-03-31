using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
    private Vector3 mouseReleasePos;
    private Rigidbody rb;
    private bool isShoot;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnMouseDown()
    {
        mouseReleasePos = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        Vector3 forceInit = (Input.mousePosition - mouseReleasePos);
        Vector3 forceV = (new Vector3(forceInit.x, forceInit.y, forceInit.z) * forceMultiplier);
        if (!isShoot)
        {
            DrawTrajectory.instance.UpdateTrajectory(forceV, rb, transform.position);     
        }
    }
    private void OnMouseUp()
    {
        mouseReleasePos = Input.mousePosition;
        Shoot(mouseReleasePos - mousePressDownPos);
    }
    [SerializeField]
    private float forceMultiplier = 2;
    void Shoot(Vector3 Force)
    {
        if (isShoot)
        {
            return;
        }
    }
}
