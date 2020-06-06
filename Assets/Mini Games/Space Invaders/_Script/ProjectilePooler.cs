using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private Transform shooterTransform;

    private List<Projectile> pooler = new List<Projectile>();

    public void ActivateProjectile(Transform shooter = null)
    {
        if(shooter != null) shooterTransform = shooter;
        try {
          Projectile projectile = pooler[0];
          pooler.RemoveAt(0);
          projectile.gameObject.SetActive(true);
          projectile.Activate(shooterTransform.position);
        } catch (Exception e) {
          Projectile newProjectile = Instantiate(prefab, shooterTransform.position,
            shooterTransform.rotation, transform);
          newProjectile.Init(this);
          newProjectile.Activate(shooterTransform.position);
        }
    }

    public void AddProjectile(Projectile projectile)
    {
      pooler.Add(projectile);
    }
}
