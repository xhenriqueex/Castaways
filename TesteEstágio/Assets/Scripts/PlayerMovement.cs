using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    // Awake player variables
    
    // Player movement variables
    NavMeshAgent agent;
    public Animator animator;
    bool isTrigger = false;
    bool awake = false;

    public Vector3 affCastaway;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Set the animator
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Awake player
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitAwake;
            Ray rayAwake = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(rayAwake, out hitAwake))
            {
                if (hitAwake.transform.tag == "Players")
                {
                    awake = true;
                    CameraController.instance.followTransform = transform;
                    affCastaway = transform.position;
                    return;
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isTrigger = false;
            animator.SetBool("isTrigger", isTrigger);
            awake = false;
            return;
        }
        // Player movement
        if (awake)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitMovement;
                Ray rayMovement = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(rayMovement, out hitMovement, Mathf.Infinity))
                {
                    agent.SetDestination(hitMovement.point);
                    affCastaway = transform.position;
                }
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isTrigger = false;
            }
            else
            {
                isTrigger = true;
            }
            animator.SetBool("isTrigger", isTrigger);
        }
    }
}
