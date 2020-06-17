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
        Debug.Log(manager != null);
        lastTimeShot = reloadTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy Projectile"))
        {
            collider.gameObject.GetComponent<Projectile>().Deactivate();
            transform.Find("Explosion").GetComponent<Explosion>().Explode();
            manager.GameOver();
        }
    }

    protected override void DoFixedUpdate()
    {
      Vector3 position = transform.position;
      GetComponent<Rigidbody2D>().MovePosition(new Vector2(position.x, position.y) +
        new Vector2(joystick.Horizontal, 0) * speed * Time.fixedDeltaTime);
      lastTimeShot += Time.fixedDeltaTime;
        Debug.Log(manager != null);
    }
    /// <summary>
    /// Shoots a projectile by getting it from a designated projectile pool.
    /// Also changes the color of the shooting button indicating that the button
    /// was pressed.
    /// </summary>
    public void Shoot()
    {
      knob.GetComponent<Image>().color = new Color(255, 138, 0, 233);
      if(lastTimeShot > reloadTime){
        pooler.ActivateProjectile();
        lastTimeShot = 0;
      }
    }
    /// <summary>
    /// Changes color of shooting button back to default color.
    /// </summary>
    public void Release()
    {
      knob.GetComponent<Image>().color = new Color(0, 255, 0, 255);
    }
}
