using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float fadeOutSpeed;

    public void Explode()
    {
      StartCoroutine(ExplosionProcess());
    }

    void Update()
    {

    }

    IEnumerator ExplosionProcess()
    {
      GameObject parentObj = transform.parent.gameObject;
      ParticleSystem explosion = GetComponent<ParticleSystem>();
      parentObj.GetComponent<Rigidbody2D>().simulated = false;
      parentObj.GetComponent<SpriteRenderer>().enabled = false;
      explosion.Play();
      while(explosion.isPlaying) yield return null;
      parentObj.SetActive(false);
    }
}
