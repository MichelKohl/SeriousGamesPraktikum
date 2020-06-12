using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : GameplayObject
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private float movement;
    [SerializeField] private float startUpdateFrequ;
    [SerializeField] private float acceleration;
    [SerializeField] private GameObject bodypart;

    private Vector3 direction;
    private float updateFrequency, counter;
    private GameObject head;
    private List<GameObject> body;
    private bool eating;

    void Awake()
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

    protected override void DoFixedUpdate()
    {
        if(counter > updateFrequency){
            UpdateDirection();
            Move();
            counter = 0;
        }
        counter += Time.fixedDeltaTime;
    }
    /// <summary>
    /// Maps joystick orientation to the move direction of the snake. Movement
    /// is limited. Snake can't move backwards.
    /// </summary>
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
    /// <summary>
    /// Moves snake forward depending on current direction.
    /// </summary>
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
    /// <summary>
    /// Function to be called, when snake head is in same position as a food object
    /// </summary>
    public void Eat()
    {
        updateFrequency /= acceleration;
        eating = true;
    }
}
