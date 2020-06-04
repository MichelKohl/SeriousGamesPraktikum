using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Vector3 moveBy;

    void OnTriggerEnter2D(Collider2D collider)
    {
      collider.gameObject.transform.position += moveBy;
    }
}
