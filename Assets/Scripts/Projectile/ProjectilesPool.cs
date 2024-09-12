using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilesPool : SingletonComponent<ProjectilesPool>
{
    [SerializeField]
    protected Projectile _projectilePrefab;

    protected ObjectPool<Projectile> _projectilesPool;

    private void Awake()
    {
        _projectilesPool = new ObjectPool<Projectile>(Create, Get, Release);
    }

    private Projectile Create()
    {
        Projectile newProjectile = Instantiate(_projectilePrefab, transform);
        newProjectile.gameObject.SetActive(false);

        return newProjectile;
    }

    private void Get(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void Release(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    public Projectile GetProjectile(Vector2 position)
    {
        var newProjectile = _projectilesPool.Get();
        newProjectile.transform.position = position;
        return newProjectile;
    }

    public void ReleaseProjectile(Projectile projectile) => _projectilesPool.Release(projectile);
}
