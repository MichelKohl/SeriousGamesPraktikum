using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private float speed;

    private float lastTimeShot;

    void FixedUpdate()
    {
    Vector3 position = transform.position;
    GetComponent<Rigidbody2D>().MovePosition(new Vector2(position.x, position.y) +
      new Vector2(joystick.Horizontal, 0) * speed * Time.fixedDeltaTime);
    }
}
