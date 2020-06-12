using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvader : GameplayObject
{
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;

    private Vector2 direction;
    private Rigidbody2D body;

    void Awake()
    {
      direction = new Vector2(1, 0);
      body = GetComponent<Rigidbody2D>();
    }

    protected override void DoUpdate()
    {
      Vector3 pos = transform.position;
      body.MovePosition(new Vector2(pos.x, pos.y) + direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      foreach(Transform row in transform.parent.transform.parent)
        foreach(Transform invader in row)
          if(invader.gameObject.activeSelf)
            invader.gameObject.GetComponent<SpaceInvader>().ChangeDirection();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
      if(collider.tag == "Friendly Fire")
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider);
      else {
        if(collider.tag == "Projectile"){
          collider.gameObject.GetComponent<Projectile>().Deactivate();
          transform.Find("Explosion").GetComponent<Explosion>().Explode();
          manager.IncreaseScoreBy(100);
        } else manager.GameOver();
      }
    }

    public void ChangeDirection()
    {
      StartCoroutine(Advance());
    }

    IEnumerator Advance()
    {
      bool right = direction.x > 0;
      direction = new Vector2(0, -1);
      yield return new WaitForSeconds(speed);
      direction = right ? -transform.right : transform.right;
    }
}
