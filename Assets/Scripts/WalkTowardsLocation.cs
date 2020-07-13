using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTowardsLocation : MonoBehaviour
{
    [SerializeField] private GameObject wayPoint;
    [SerializeField] private Animator AnimationController;
    private Vector3 LocationObjectPos;
    [SerializeField] private float walkSpeed = 2.5f;
    
    void Update()
    {
        LocationObjectPos = new Vector3(wayPoint.transform.position.x, transform.position.y, wayPoint.transform.position.z);
        //transform.position = Vector3.MoveTowards(transform.position, wayPointPos, walkSpeed * Time.deltaTime);
        if (Vector3.Distance(LocationObjectPos, transform.position) > 0.1f)
        {
            AnimationController.SetBool("Walk", true);
            transform.position = Vector3.MoveTowards(transform.position, LocationObjectPos, walkSpeed * Time.deltaTime);
        }
        else
        {
            AnimationController.SetBool("Walk", false);
           // transform.position = Vector3.MoveTowards(transform.position, wayPointPos, walkSpeed * Time.deltaTime);
        }

    }
}
