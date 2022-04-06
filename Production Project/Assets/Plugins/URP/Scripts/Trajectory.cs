using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField]List<Vector3> pathVerysList = new List<Vector3>();
    [SerializeField] float time = 1;
    [SerializeField] Vector3 velocity = Vector3.right;
    [SerializeField] Vector3 acceleration = Vector3.down;
    [SerializeField] int splits = 3;
    // UT+.5 ATT
    public float Distance_InTime_DueToAcc(float a, float t)
    {
        return 0.5f * a * t * t;
    }
    public void Calculate_Trajectory()
    {
        if (pathVerysList == null)
        {
            pathVerysList = new List<Vector3>();
        }
        pathVerysList.Clear();
        float dt = 0;
        for (int i = 0; i < splits; i++)
        {
            dt = (time / (splits - 1)) * i;
            Vector3 d = Vector3.up * Distance_InTime_DueToAcc(acceleration.y, dt);
            d.x = velocity.x * dt;
            pathVerysList.Add(d);
        }
    }

    [Header("Ref")]
    [SerializeField] LineRenderer LineRendere = null;


    [Header("Ediotr Setting")]
    [SerializeField] bool calc_Trajectory = false;
    [SerializeField] bool auto_calc = false;
    private void OnDrawGizmosSelected()
    {
        if (calc_Trajectory || auto_calc)
        {
            calc_Trajectory = false;
            Calculate_Trajectory();
            LineRendere.positionCount = splits;
            LineRendere.SetPositions(pathVerysList.ToArray());
        }
    }
}
