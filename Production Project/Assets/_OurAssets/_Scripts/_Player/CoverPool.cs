using System.Collections.Generic;
using UnityEngine;

public class CoverPool : MonoBehaviour
{

    public static Queue<GameObject> ReadyToFireCoversQueue = new Queue<GameObject>();
    public static Queue<GameObject> ActiveCoversQueue = new Queue<GameObject>();
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private int _poolStartSize = 3;


    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject wall = Instantiate(_wallPrefab);
            wall.SetActive(false);
        }

        ActiveCoversQueue.Clear();
    }

    public static GameObject GetCoverFromPool()
    {
        if (ReadyToFireCoversQueue.Count > 0)
        {
            GameObject cover = ReadyToFireCoversQueue.Dequeue();
            cover.SetActive(true);
            return cover;
    }
        else if (ReadyToFireCoversQueue.Count <= 0 && ActiveCoversQueue.Count > 0)
        {
            ForcePullCover();

            GameObject cover = ReadyToFireCoversQueue.Dequeue();
            cover.SetActive(true);
            return cover;
        }
        else
        {
            Debug.LogError("Pools are empty, F in chat");
            return null;
        }
    }

    private static void ForcePullCover()
    {
        GameObject cover;

        for (int i = 0; i < ActiveCoversQueue.Count; i++)
        {
            if ((cover = ActiveCoversQueue.Dequeue()).activeSelf)
            {
                cover.SetActive(false);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Card");
                break;
            }
        }
    }

}