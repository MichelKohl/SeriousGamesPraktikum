using UnityEngine;

public class Bodypart : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject.Find("GameManager").GetComponent<SnakeGameManager>().GameOver();
    }
}
