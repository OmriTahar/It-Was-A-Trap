using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Queue<GameObject> _projectilePool = new Queue<GameObject>();
    [SerializeField] int _poolStartSize = 20;

    void Start()
    {
        for (int i = 0; i < _poolStartSize; i++)
        {
            GameObject projectile = Instantiate(_projectilePrefab);
            _projectilePool.Enqueue(projectile);

            projectile.GetComponent<CarrotProjectile>().SetMe(this);

            projectile.SetActive(false);
        }
    }

    public GameObject GetProjectileFromPool()
    {
        if (_projectilePool.Count > 0)
        {
            GameObject projectile = _projectilePool.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        else
        {
            GameObject projectile = Instantiate(_projectilePrefab);
            return projectile;
        }
    }

    public void ReturnProjectileToPool(GameObject projectile)
    {
        _projectilePool.Enqueue(projectile);
        projectile.SetActive(false);
    }
}
