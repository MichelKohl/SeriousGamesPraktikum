using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float timeToDie;
    [SerializeField] private float verticalOffset;

    private ProjectilePooler pooler;
    private float timeAlive;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
      timeAlive = 0;
    }

    void Awake()
    {
      rb = GetComponent<Rigidbody2D>();
      sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
      rb.MovePosition(transform.position + transform.up * speed * Time.fixedDeltaTime);
      if(timeAlive > timeToDie) {
        pooler.AddProjectile(this);
        Deactivate();
      }
    }

    public void Init(ProjectilePooler pooler)
    {
      this.pooler = pooler;
    }

    public void Activate(Vector3 position)
    {
      rb.simulated = true;
      sr.enabled = true;
      transform.position = verticalOffset != 0 ? position + new Vector3(0, verticalOffset, 0) : position;
      timeAlive = 0;
    }

    public void Deactivate()
    {
      rb.simulated = false;
      sr.enabled = false;
      transform.position = new Vector3(0, 0, 200);
      pooler.AddProjectile(this);
      gameObject.SetActive(false);
    }
}
