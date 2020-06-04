using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private float movement;
    [SerializeField] private float startUpdateFrequ;
    [SerializeField] private GameObject bodypart;

    private Vector3 direction;
    private float updateFrequency, counter;
    private GameObject head;
    private List<GameObject> body;
    private bool eating;
    // Start is called before the first frame update
    void Start()
    {
      counter = 0;
      eating = false;
      updateFrequency = startUpdateFrequ;
      direction = new Vector3(0,1,0);
      head = GameObject.Find("Head");
      body = new List<GameObject>();
      body.Add(GameObject.Find("Bodypart 2"));
      body.Add(GameObject.Find("Bodypart 1"));
    }

    void FixedUpdate()
    {
      if(counter > updateFrequency){
        UpdateDirection();
        Move();
        counter = 0;
      }
      counter += Time.fixedDeltaTime;
    }
    void UpdateDirection()
    {
      float horizontal = joystick.Horizontal, vertical = joystick.Vertical;
      if(Math.Abs(horizontal) > Math.Abs(vertical))
        direction = new Vector3(horizontal < 0 ? -1 : 1, 0, 0);
      if(Math.Abs(horizontal) < Math.Abs(vertical))
        direction = new Vector3(0, vertical < 0 ? -1 : 1, 0);
      // snake cannot go backwards...
      if(direction + head.transform.up == new Vector3(0,0,0))
        direction = head.transform.up;
    }

    void Move()
    {
      Transform headTransform = head.transform;
      if(eating){
        eating = false;
        body.Add(Instantiate(bodypart, headTransform.position, headTransform.rotation, transform));
      } else {
        GameObject lastBodypart = body[0];
        lastBodypart.transform.position =  headTransform.position;
        lastBodypart.transform.up = headTransform.up;
        body.RemoveAt(0);
        body.Add(lastBodypart);
      }
      head.transform.position += movement * direction;
      head.transform.up = direction;
    }

    public void Eat()
    {
      eating = true;
    }
}
