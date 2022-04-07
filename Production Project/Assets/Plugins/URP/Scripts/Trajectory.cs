using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] List<Vector3> pathVerysList = new List<Vector3>();
    [SerializeField] float time = 1;
    [SerializeField] Vector3 velocity = Vector3.forward;
    [SerializeField] Vector3 acceleration = Vector3.down;
    [SerializeField] Vector3 unityAccuracyFix = Vector3.zero;
    [SerializeField] int splits = 3;
    // UT+.5 ATT
    public float Distance_InTime_DueToAcc(float u, float a, float t)
    {
        return u * t + 0.5f * a * t * t;
    }
    public void Calculate_Trajectory()
    {
        if (pathVerysList == null)
        {
            pathVerysList = new List<Vector3>();
        }
        pathVerysList.Clear();
        Vector3 d;
        float dt = 0;
        for (int i = 0; i < splits; i++)
        {
            dt = (time / (splits - 1)) * i;
            d.x = Distance_InTime_DueToAcc(velocity.x, acceleration.x, dt);
            d.y = Distance_InTime_DueToAcc(velocity.y, acceleration.y, dt);
            d.z = Distance_InTime_DueToAcc(velocity.z, acceleration.z, dt);
            pathVerysList.Add(d);
        }
    }

    [Header("Ref")]
    [SerializeField] LineRenderer LineRendere = null;
    [SerializeField] Rigidbody projectile;


    [Header("Ediotr Setting")]
    [SerializeField] bool calc_Trajectory = false;
    [SerializeField] bool auto_calc = false;

    [Space]
    [SerializeField] bool fire = false;
    private void OnDrawGizmosSelected()
    {
        if (calc_Trajectory || auto_calc)
        {
            calc_Trajectory = false;
            Calculate_Trajectory();
            LineRendere.positionCount = splits;
            LineRendere.SetPositions(pathVerysList.ToArray());
            projectile.transform.position = LineRendere.transform.position;
            print("i set line renderer to postions from array");
        }

        if (fire)
        {
            fire = false;

            projectile.transform.position = transform.position;
            projectile.velocity = velocity + unityAccuracyFix;
        }
    }
}
