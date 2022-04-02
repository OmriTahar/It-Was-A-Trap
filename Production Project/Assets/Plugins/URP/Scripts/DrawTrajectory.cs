using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    [Range(3, 30)]
    private int _linesegmentCount = 20;
    private List<Vector3> _linePoints = new List<Vector3>();
    [SerializeField]
    [Range(10, 100)]
    private int _showPercentage = 50;
    [SerializeField]
    private int _linePointsCount;
    #region Singleton
    public static DrawTrajectory instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    public void UpdateTrajectory (Vector3 forceVector,Rigidbody rigidbody,Vector3 startingPoint)
    {
        Vector3 velocity = (forceVector / rigidbody.mass) * Time.fixedDeltaTime;
        float stepTime = FlightDuration / _linesegmentCount;
        _linePoints.Clear();
        for (int i = 0; i < _linePointsCount; i++)
        {
            float stepTimePassed = stepTime * i;//Change in time
            Vector3 MovementVector = new Vector3(
             x:velocity.x * stepTimePassed,
             y:velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
             z:velocity.z * stepTimePassed
                );
            RaycastHit hit;
            if (Physics.Raycast(origin:startingPoint,direction:-MovementVector,out hit,MovementVector.magnitude))
            {
                break;
            }
            _linePoints.Add(item: -MovementVector + startingPoint);
        }
        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());
    }
}
