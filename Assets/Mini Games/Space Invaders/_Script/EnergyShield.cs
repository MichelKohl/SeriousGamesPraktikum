using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShield : MonoBehaviour
{
    private int health;
    // Start is called before the first frame update
    void Start()
    {
      health = 3;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
      health--;
      collider.gameObject.GetComponent<Projectile>().Deactivate();
      SpriteRenderer circle = transform.Find("Circle").GetComponent<SpriteRenderer>();
      SpriteRenderer bubble = transform.Find("Bubble").GetComponent<SpriteRenderer>();
      switch(health){
        case 2:
          circle.color = new Color(255,255,0, circle.color.a);
          bubble.color = new Color(255,255,0, bubble.color.a);
          break;
        case 1:
          circle.color = new Color(255,0,0, circle.color.a);
          bubble.color = new Color(255,0,0, bubble.color.a);
          break;
        case 0:
          circle.color = new Color(0,0,0,0);
          bubble.color = new Color(0,0,0,0);
          gameObject.SetActive(false);
          break;
        default: break;
      }
    }
}
