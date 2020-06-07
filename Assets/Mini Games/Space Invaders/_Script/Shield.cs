using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
      collider.gameObject.GetComponent<Projectile>().Deactivate();
      transform.Find("Explosion").GetComponent<Explosion>().Explode();
    }
}
