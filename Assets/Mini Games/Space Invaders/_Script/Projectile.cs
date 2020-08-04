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
    /// <summary>
    /// Initialized projectile. Binds it to a projectile pooler.
    /// </summary>
    /// <param name="pooler">Projectile pooler the projectile should be bound to</param>
    public void Init(ProjectilePooler pooler)
    {
        this.pooler = pooler;
    }
    /// <summary>
    /// Activates a projectile.
    /// </summary>
    /// <param name="position">Position where the projectile should be fired from</param>
    public void Activate(Vector3 position)
    {
        rb.simulated = true;
        sr.enabled = true;
        transform.position = verticalOffset != 0 ? position + new Vector3(0, verticalOffset, 0) : position;
        timeAlive = 0;
    }
    /// <summary>
    /// Deactivates a projectile. Should be called if a collision occured.
    /// </summary>
    public void Deactivate()
    {
        rb.simulated = false;
        sr.enabled = false;
        transform.position = new Vector3(0, 0, 200);
        pooler.AddProjectile(this);
        gameObject.SetActive(false);
    }
}
