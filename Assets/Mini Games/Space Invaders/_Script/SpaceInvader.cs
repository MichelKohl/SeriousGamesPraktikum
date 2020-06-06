using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvader : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;

    private Vector2 direction;
    private Rigidbody2D body;

    void Start()
    {
      direction = new Vector2(1, 0);
    }

    void Awake()
    {
      body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
      Vector3 pos = transform.position;
      body.MovePosition(new Vector2(pos.x, pos.y) + direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      string name = collision.gameObject.name;
      if(name == "Border")
        foreach(Transform row in transform.parent.transform.parent)
          foreach(Transform invader in row)
            invader.gameObject.GetComponent<SpaceInvader>().ChangeDirection();
      if(name == "Game Over Border")
        GameObject.Find("GameManager").GetComponent<SpaceInvaderGameManager>().GameOver();
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
