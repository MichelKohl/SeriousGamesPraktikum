using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private ParticleSystem explosion;

    private Transform targetTransform;

    public void LockOnTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null)
            transform.Translate((targetTransform.position - transform.position) * Time.deltaTime * speed);
    }

    public void DoExplosion()
    {
        StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        //TODO do explosion
        if(explosion != null)
        {
            ParticleSystem newExplosion = Instantiate(explosion, transform.position, Quaternion.identity, transform.parent);
            transform.position -= new Vector3(0, -100, 0);
            newExplosion.Play();
            yield return new WaitForSeconds(newExplosion.main.duration);
            Destroy(newExplosion.gameObject);
        }
        Destroy(gameObject);
    }
}
