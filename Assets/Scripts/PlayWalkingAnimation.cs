using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWalkingAnimation : MonoBehaviour
{
    [SerializeField] private Animator AnimationController;
    private GameObject player = null;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            lastPosition = player.transform.position;
        }
        InvokeRepeating("checkWalk", 2.0f, 1.0f);
    }

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
