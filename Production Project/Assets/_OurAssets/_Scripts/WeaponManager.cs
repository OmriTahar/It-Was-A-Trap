using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform PlayerTransform;
    [SerializeField] GameObject CirclePrefab;
    [SerializeField] GameObject LinePrefab;
    [SerializeField] protected float Range = 10;
    [SerializeField] protected int Damage = 5;

    private void Start()
    {
        //Instantiate(CirclePrefab, PlayerTransform.position.normalized * Range, Quaternion.identity, PlayerTransform);
        //Instantiate(LinePrefab, PlayerTransform);
    }

    private void Update()
    {
        
    }

    public virtual void Fire(GameObject ShotPrefab)
    {

    }

}