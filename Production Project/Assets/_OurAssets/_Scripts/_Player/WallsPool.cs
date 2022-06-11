using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsPool : MonoBehaviour
{
    [SerializeField] GameObject _wallPrefab;
    public Queue<GameObject> WallQueue = new Queue<GameObject>();
    [SerializeField] int _poolStartSize = 3;

    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject wall = Instantiate(_wallPrefab);
            WallQueue.Enqueue(wall);
            wall.GetComponent<Wall>().SetMe(this);
            wall.SetActive(false);
        }
    }

    public GameObject GetProjectileFromPool()
    {
        if (WallQueue.Count > 0)
        {
            GameObject wall = WallQueue.Dequeue();
            wall.SetActive(true);
            return wall;
        }
        else
        {
            print("Pool is currently empty");
            return null;
        }
    }

    public void ReturnWallToPool(GameObject wall)
    {
        WallQueue.Enqueue(wall);
        wall.SetActive(false);
    }
}