using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private Transform shooterTransform;
    [SerializeField] private bool isEnemyPooler;

    private List<Projectile> pooler = new List<Projectile>();

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

    public void AddProjectile(Projectile projectile)
    {
      pooler.Add(projectile);
    }
}
