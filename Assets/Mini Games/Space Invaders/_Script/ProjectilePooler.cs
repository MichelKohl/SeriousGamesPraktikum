using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private Transform shooterTransform;
    [SerializeField] private bool isEnemyPooler;

    private List<Projectile> pooler = new List<Projectile>();
    /// <summary>
    /// Activates a projectile and removes it from current pool of deactivated
    /// projectiles.
    /// </summary>
    /// <param name="shooter">Transform object to get shooting position</param>
    public void ActivateProjectile(Transform shooter = null)
    {
        if(shooter != null) shooterTransform = shooter;
        try
        {
            Projectile projectile = pooler[0];
            pooler.RemoveAt(0);
            projectile.gameObject.SetActive(true);
            projectile.Activate(shooterTransform.position);
        } catch (Exception)
        {
            Quaternion rotation = isEnemyPooler ? Quaternion.Euler(0, 0, 180) : shooterTransform.rotation;
            Projectile newProjectile = Instantiate(prefab, shooterTransform.position, rotation, transform);
            newProjectile.Init(this);
            newProjectile.Activate(shooterTransform.position);
        }
    }
    /// <summary>
    /// Adds a projectile to pool of deactivated projectiles.
    /// </summary>
    /// <param name="projectile">Projectile to be added</param>
    public void AddProjectile(Projectile projectile)
    {
        pooler.Add(projectile);
    }
}
