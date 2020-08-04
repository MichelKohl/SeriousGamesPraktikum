using UnityEngine;

public class Bodypart : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        transform.parent.GetComponent<Snake>().EatSelf();
    }
}
