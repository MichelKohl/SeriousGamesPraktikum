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
      switch(collision.gameObject.name)
      {
        case "Border":
        foreach(Transform row in transform.parent.transform.parent)
          foreach(Transform invader in row)
            if(invader.gameObject.activeSelf)
              invader.gameObject.GetComponent<SpaceInvader>().ChangeDirection();
        break;
        case "Game Over Border":
        manager.GameOver();
        break;
        default:
        Physics2D.IgnoreCollision( GetComponent<Collider2D>(),
              collision.gameObject.GetComponent<Collider2D>());
        break;
      }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
      if(collider.tag == "Friendly Fire")
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider);
      else {
        collider.gameObject.GetComponent<Projectile>().Deactivate();
        transform.Find("Explosion").GetComponent<Explosion>().Explode();
        manager.IncreaseScoreBy(100);
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
