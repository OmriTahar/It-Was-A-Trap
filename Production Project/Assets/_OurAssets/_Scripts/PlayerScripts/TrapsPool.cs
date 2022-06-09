using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsPool : MonoBehaviour
{
    [SerializeField] GameObject _trapPrefab;
    public Queue<GameObject> TrapPoolQueue = new Queue<GameObject>();
    [SerializeField] int _poolStartSize = 20;

    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject trap = Instantiate(_trapPrefab);
            TrapPoolQueue.Enqueue(trap);
            trap.GetComponent<Trap>().SetMe(this);
            trap.SetActive(false);
        }
    }

    public GameObject GetProjectileFromPool()
    {
        if (TrapPoolQueue.Count > 0)
        {
            GameObject projectile = TrapPoolQueue.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else
        {
            print("Pool is currently empty");
            return null;
        }
    }

    public void ReturnTrapToPool(GameObject trap)
    {
        TrapPoolQueue.Enqueue(trap);
        trap.SetActive(false);
    }
}
