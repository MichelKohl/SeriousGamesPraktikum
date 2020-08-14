using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationTester : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        //agent.SetDestination(transform.position - new Vector3(-2, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("sword attack");
            animator.SetTrigger("doSimpleAttack");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("sword attack");
            animator.SetTrigger("doSimpleAttack2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("jump slash");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("cast spell");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetTrigger("sword spell");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("power up");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("isDead", true);
            animator.SetFloat("random", Random.Range(0, 1f));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("spell");
        }


        animator.SetFloat("movementSpeed", -Mathf.Sqrt(Mathf.Pow(agent.velocity.x, 2) + Mathf.Pow(agent.velocity.y, 2) + Mathf.Pow(agent.velocity.z, 2)));
    }
}
