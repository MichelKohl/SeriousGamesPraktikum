using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to play a walking animation for the player
/// </summary>
public class PlayWalkingAnimation : MonoBehaviour
{
    [SerializeField] private Animator AnimationController;
    [SerializeField] private float InitalWaitForWalking = 0.0f;
    [SerializeField] private float UpdateWalking = 0.5f;
    private GameObject player = null;
    private Vector3 lastPosition;

    /// <summary>
    /// Start is called before the first frame update. Invokes repeating function to trigger walking animation
    /// </summary>
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            lastPosition = player.transform.position;
        }
        InvokeRepeating("checkWalk", InitalWaitForWalking, UpdateWalking);
    }

    /// <summary>
    /// Checks if the player location has changed to trigger the walking animation
    /// </summary>
    void checkWalk()
    {
        if (Vector3.Distance(lastPosition, player.transform.position) > Vector3.kEpsilon)
        {
            AnimationController.SetBool("Walk", true);
        }
        else
        {
            AnimationController.SetBool("Walk", false);
        }
        lastPosition = player.transform.position;
        
    }
}
