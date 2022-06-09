using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsPool : MonoBehaviour
{
    [SerializeField] GameObject _wallPrefab;
    public Queue<GameObject> WallPoolQueue = new Queue<GameObject>();
    [SerializeField] int _poolStartSize = 20;

    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject wall = Instantiate(_wallPrefab);
            WallPoolQueue.Enqueue(wall);
            wall.GetComponent<Wall>().SetMe(this);
            wall.SetActive(false);
        }
    }

    public GameObject GetProjectileFromPool()
    {
        if (WallPoolQueue.Count > 0)
        {
            GameObject wall = WallPoolQueue.Dequeue();
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
        WallPoolQueue.Enqueue(wall);
        wall.SetActive(false);
    }
}
