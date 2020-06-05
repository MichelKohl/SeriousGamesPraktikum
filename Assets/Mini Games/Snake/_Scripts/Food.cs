using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private int points;
    [SerializeField] private GameObject borders;
    [SerializeField] private float offset;

    private float minX = 0, minY = 0;
    private int xUnits = 0, yUnits = 0;
    // Start is called before the first frame update
    void Start()
    {
      float maxX = 0, maxY = 0;
      foreach(Transform borderTransform in borders.transform){
        Vector3 position = borderTransform.position;
        minX = Mathf.Min(minX, position.x);
        minY = Mathf.Min(minY, position.y);
        maxX = Mathf.Max(maxX, position.x);
        maxY = Mathf.Max(maxY, position.y);
      }
      xUnits = (int) ((maxX - minX) / offset);
      yUnits = (int) ((maxY - minY) / offset);
    }

    private void OnTriggerEnter2D(Collider2D collider){
      transform.position = GetRandomPosition();
      collider.gameObject.transform.parent.gameObject.GetComponent<Snake>().Eat();
      GameObject.Find("GameManager").GetComponent<GameManager>().IncreaseScoreBy(points);
    }

    private Vector3 GetRandomPosition()
    {
      return new Vector3( minX + Mathf.FloorToInt(Random.Range(1, xUnits)) * offset,
                          minY + Mathf.FloorToInt(Random.Range(1, yUnits)) * offset, 0);
    }
}
