using System.Collections.Generic;
using UnityEngine;

public class WallsPool : MonoBehaviour
{
    public static Queue<GameObject> ReadyToFireWallsQueue = new Queue<GameObject>();
    public static Queue<GameObject> ActiveWallsQueue = new Queue<GameObject>();
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private int _poolStartSize = 3;

    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject wall = Instantiate(_wallPrefab);
            wall.SetActive(false);
        }

        ActiveWallsQueue.Clear();
    }

    public static GameObject GetWallFromPool()
    {
        if (ReadyToFireWallsQueue.Count > 0)
        {
            GameObject wall = ReadyToFireWallsQueue.Dequeue();
            wall.SetActive(true);
            return wall;
        }
        else if (ReadyToFireWallsQueue.Count <= 0 && ActiveWallsQueue.Count > 0)
        {
            ForcePullWall();

            GameObject wall = ReadyToFireWallsQueue.Dequeue();
            wall.SetActive(true);
            return wall;
        }
        else
        {
            Debug.LogError("Pools are empty, F in chat");
            return null;
        }
    }

    private static void ForcePullWall()
    {
        GameObject wall;

        for (int i = 0; i < ActiveWallsQueue.Count; i++)
        {
            if ((wall = ActiveWallsQueue.Dequeue()).activeSelf)
            {
                wall.SetActive(false);
                break;
            }
        }
    }

}