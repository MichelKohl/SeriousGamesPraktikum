using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipController : GameplayObject
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private float speed;
    [SerializeField] private ProjectilePooler pooler;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject knob;

    private float lastTimeShot;

    void Awake()
    {
      lastTimeShot = reloadTime;
    }

    protected override void DoUpdate()
    {
      Vector3 position = transform.position;
      GetComponent<Rigidbody2D>().MovePosition(new Vector2(position.x, position.y) +
        new Vector2(joystick.Horizontal, 0) * speed * Time.fixedDeltaTime);
      lastTimeShot += Time.fixedDeltaTime;
    }

    public void Shoot()
    {
      knob.GetComponent<Image>().color = new Color(255, 138, 0, 233);
      if(lastTimeShot > reloadTime){
        pooler.ActivateProjectile();
        lastTimeShot = 0;
      }
    }

    public void Release()
    {
      knob.GetComponent<Image>().color = new Color(0, 255, 0, 255);
    }
}
