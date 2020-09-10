using System.Collections;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private ParticleSystem explosion;

    private Transform targetTransform;

    public void LockOnTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
        transform.forward = Vector3.Normalize(targetTransform.position - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null)
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed);
        else Debug.Log("target is null.");
    }

    public void DoExplosion(float delay = 0)
    {
        StartCoroutine(Explode(delay));
    }

    private IEnumerator Explode(float delay)
    {
        yield return new WaitForSeconds(delay);
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
