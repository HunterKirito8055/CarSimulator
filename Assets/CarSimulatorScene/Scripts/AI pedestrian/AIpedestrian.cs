using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIpedestrian : MonoBehaviour
{
    public float movementSpeed;
    public int rotationSpeed;
    public float stopDistance;
    public bool reachedDestination;
    [SerializeField] Vector3 destination;
    [SerializeField] Animator animator;
    bool walkStop;

    public bool WalkStop
    {
        get { return walkStop; }
        set
        {
            walkStop = value;
            animator.SetBool(StringConstants.walkStop_Anim, WalkStop);
        }
    }

    void Update()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;
            if (destinationDistance >= stopDistance && !WalkStop)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }

        }
        else
        {
            reachedDestination = true;
        }
    }
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}
