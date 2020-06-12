using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float fadeOutSpeed;

    public void Explode()
    {
        StartCoroutine(ExplosionProcess());
    }
    /// <summary>
    /// Coroutine that plays the explosion and deactivates parent object.
    /// </summary>
    /// <returns>Explosion coroutine</returns>
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
