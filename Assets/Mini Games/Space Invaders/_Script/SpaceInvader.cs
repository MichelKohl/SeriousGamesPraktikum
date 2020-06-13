using System.Collections;
using UnityEngine;

public class SpaceInvader : GameplayObject
{
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] int shootProbabilty;


    private Vector2 direction;
    private Rigidbody2D body;
    private ProjectilePooler pooler;
    private float lastTimeShot;
    private float lastTimeDirectionChanged;
    private int points;

    void Awake()
    {
        direction = new Vector2(1, 0);
        body = GetComponent<Rigidbody2D>();
        lastTimeShot = Random.Range(0, 1);
        lastTimeDirectionChanged = 0;
        pooler = GameObject.Find("Projectile Pooler").transform.Find("Enemies").GetComponent<ProjectilePooler>();

        int currentLevel = PlayerPrefs.GetInt("Space Invaders Level", 1);
        points = 100 * currentLevel;
        acceleration += 0.05f * currentLevel;
    }

    protected override void DoFixedUpdate()
    {
        Vector3 pos = transform.position;
        body.MovePosition(new Vector2(pos.x, pos.y) + direction * speed * Time.deltaTime);
        lastTimeShot = lastTimeShot > 1 ? 0 : lastTimeShot + Time.fixedDeltaTime;
        lastTimeDirectionChanged += Time.fixedDeltaTime;
        if (Random.Range(0, 100) <= shootProbabilty && lastTimeShot > 1)
            pooler.ActivateProjectile(transform);
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
        switch (collider.tag)
        {
            case "Enemy Projectile":
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider);
            break;
            case "Projectile":
                collider.gameObject.GetComponent<Projectile>().Deactivate();
                transform.Find("Explosion").GetComponent<Explosion>().Explode();
                manager.IncreaseScoreBy(points);
                ((SpaceInvaderGameManager)manager).IncreaseKillCount();
            break;
            case "Game Over":
                manager.GameOver();
            break;
            default:
            break;
        }
    }
    /// <summary>
    /// Function used to toggle current direction of the space invaders.
    /// </summary>
    public void ChangeDirection()
    {
        if (lastTimeDirectionChanged < 1) return;
        speed *= acceleration;
        StartCoroutine(Advance());
        lastTimeDirectionChanged = 0;
    }
    /// <summary>
    /// Coroutine used by function above.
    /// </summary>
    /// <returns>Coroutine, that changes direction of the space invaders</returns>
    IEnumerator Advance()
    {
        bool right = direction.x > 0;
        direction = new Vector2(0, -1);
        yield return new WaitForSeconds(speed);
        direction = right ? -transform.right : transform.right;
    }
}
