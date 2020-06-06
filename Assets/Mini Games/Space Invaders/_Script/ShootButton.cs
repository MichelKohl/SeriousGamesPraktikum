using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootButton : MonoBehaviour
{
    [SerializeField] private ProjectilePooler pooler;
    [SerializeField] private float reloadTime;

    private float lastTimeShot;

    void Start()
    {
      lastTimeShot = reloadTime;
    }

    public void Shoot()
    {
      GetComponent<Image>().color = new Color(255, 138, 0, 233);
      if(lastTimeShot > reloadTime){
        pooler.ActivateProjectile();
        lastTimeShot = 0;
      }
    }

    public void Release()
    {
      GetComponent<Image>().color = new Color(0, 255, 0, 255);
    }

    void FixedUpdate()
    {
      lastTimeShot += Time.fixedDeltaTime;
    }
}
