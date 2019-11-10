using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    PATROLLING, // Patrols room for 10 seconds
    SEEKING, // Is approaching the agent to attack
    ATTACKING, // Is attacking the agent
    DEAD // Agent won, no longer moving, change to dead sprite
}

public class EnemyAgentScript : MonoBehaviour
{
    public List<Transform> patrolPoints;
    int currentPatrolPoint = 1;
    Vector3 steeringVelocity = Vector3.zero;
    Vector3 desiredVelocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get the target position from the mouse input
		Vector3 targetPosition = patrolPoints[currentPatrolPoint].position;

		// Get the desired velocity for seek and limit to maxSpeed
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * 0.05f;

        transform.position += desiredVelocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        currentPatrolPoint++;
        if (currentPatrolPoint >= patrolPoints.Count)
        {
            currentPatrolPoint = 0;
        }

        this.transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, -90));
    }
}
