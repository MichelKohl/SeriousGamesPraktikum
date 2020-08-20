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
            transform.Translate(Vector3.Normalize(targetTransform.position - transform.position)
                * Time.deltaTime * speed);
    }

    public void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        //TODO do explosion
        if(explosion != null)
        {
            // start explosion
            // yield return new WaitUntil(() => exposion.done);
        }
        yield return null;
        Destroy(gameObject);
    }
}
