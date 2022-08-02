using System.Collections.Generic;
using UnityEngine;

public class TrapsPool : MonoBehaviour
{
    public static Queue<GameObject> ReadyToFireTrapsQueue = new Queue<GameObject>();
    public static Queue<GameObject> ActiveTrapsQueue = new Queue<GameObject>();
    [SerializeField] private GameObject _trapPrefab;
    [SerializeField] private int _poolStartSize = 5;

    void Start()
    {
        ReadyToFireTrapsQueue.Clear();

        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject trap = Instantiate(_trapPrefab);
            trap.SetActive(false);
        }

        ActiveTrapsQueue.Clear();
    }

    public static GameObject GetTrapFromPool()
    {
        if (ReadyToFireTrapsQueue.Count > 0)
        {
            GameObject projectile = ReadyToFireTrapsQueue.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else if (ReadyToFireTrapsQueue.Count <= 0 && ActiveTrapsQueue.Count > 0)
        {
            ForcePullProjectile();

            GameObject projectile = ReadyToFireTrapsQueue.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else
        {
            Debug.LogError("Pools are empty, F in chat");
            return null;
        }
    }

    private static void ForcePullProjectile()
    {
        GameObject activeTrap;

        for (int i = 0; i < ActiveTrapsQueue.Count; i++)
        {
            if ((activeTrap = ActiveTrapsQueue.Dequeue()).activeSelf)
            {
                activeTrap.SetActive(false);
                break;
            }
        }
    }
}