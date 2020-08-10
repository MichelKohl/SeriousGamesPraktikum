using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lets player character walk towards location object which is controlled by gps location
/// </summary>
public class WalkTowardsLocation : MonoBehaviour
{
    [SerializeField] 
    private GameObject wayPoint;
    [SerializeField]
    private Animator AnimationController;
    private Vector3 LocationObjectPos;
    [SerializeField] 
    private float walkSpeed = 2.5f;
    [SerializeField] 
    private float distanceMultiplier = 0.2f;

    void Update()
    {
        var distFactor = 1.0f;
        LocationObjectPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
        if (Vector3.Distance(LocationObjectPos, transform.position) > 0.1f)
        {
            if (!AnimationController.GetBool("Walk") && Vector3.Distance(LocationObjectPos, transform.position) > 0.2f)
            {
                AnimationController.SetBool("Walk", true);
            }     
            distFactor =Math.Max(1.0f, Vector3.Distance(LocationObjectPos, transform.position) * distanceMultiplier);
            transform.position = Vector3.MoveTowards(transform.position, LocationObjectPos, distFactor * walkSpeed * Time.deltaTime);
        }
        else
        {
            if (AnimationController.GetBool("Walk"))
            {
                AnimationController.SetBool("Walk", false);
            }           
        }
        
    }
}
