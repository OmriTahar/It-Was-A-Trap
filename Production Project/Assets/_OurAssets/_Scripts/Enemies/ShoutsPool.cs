using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoutsPool : MonoBehaviour
{

    [SerializeField] GameObject _shoutPrefab;
    [SerializeField] Queue<GameObject> _shoutPoolQueue = new Queue<GameObject>();
    [SerializeField] int _poolStartSize = 10;


    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject shout = Instantiate(_shoutPrefab);
            _shoutPoolQueue.Enqueue(shout);
            shout.GetComponent<ShoutAttack>().SetMe(this);
            shout.SetActive(false);
        }
    }

    public GameObject GetShoutFromPool()
    {
        if (_shoutPoolQueue.Count > 0)
        {
            GameObject shout = _shoutPoolQueue.Dequeue();
            shout.SetActive(true);
            return shout;
        }
        else
        {
            print("Pool is currently empty");
            return null;
        }
    }

    public void ReturnProjectileToPool(GameObject shout)
    {
        _shoutPoolQueue.Enqueue(shout);
        shout.SetActive(false);
    }
}
