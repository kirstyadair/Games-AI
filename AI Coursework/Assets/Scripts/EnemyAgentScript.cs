using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    PATROLLING, // Patrols room
    SEEKING, // Is approaching the agent to attack
    ATTACKING, // Is attacking the agent
    DEAD // Agent won, no longer moving, change to dead sprite
}

public class EnemyAgentScript : MonoBehaviour
{
    public List<Transform> patrolPoints;
    public EnemyStates state;
    public GameObject agent;
    public Sprite dead;
    public float viewDistance;
    public float attackDistance;
    public int hitsRemaining;

    Vector3 steeringVelocity = Vector3.zero;
    Vector3 desiredVelocity;
    int currentPatrolPoint = 1;
    float hitCooldown = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        state = EnemyStates.PATROLLING;
    }



    // Update is called once per frame
    void Update()
    {
        if (state == EnemyStates.PATROLLING)
        {
            Patrol();
        }
        else if (state == EnemyStates.SEEKING)
        {
            Seek();
        }
        else if (state == EnemyStates.ATTACKING)
        {
            Attack();
        }
        else if (state == EnemyStates.DEAD)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = dead;
        }
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



    void Patrol()
    {
        // Target position is the next point in the patrolPoints list
		Vector3 targetPosition = patrolPoints[currentPatrolPoint].position;
		// Get the desired velocity and multiply by speed
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * 0.05f;
        // Move in the desired direction
        transform.position += desiredVelocity;

        // Check if the agent is within viewing distance of the enemy
        float distanceBetweenAgents = Vector3.Distance(agent.transform.position, gameObject.transform.position);
        if (distanceBetweenAgents <= viewDistance)
        {
            state = EnemyStates.SEEKING;
        }
    }



    void Seek()
    {
        desiredVelocity = Vector3.Normalize(agent.transform.position - transform.position) * 0.05f;
        transform.position += desiredVelocity;

        // Check if the agent is within viewing distance of the enemy
        float distanceBetweenAgents = Vector3.Distance(agent.transform.position, gameObject.transform.position);
        if (distanceBetweenAgents >= viewDistance)
        {
            state = EnemyStates.PATROLLING;
        }
        else if (distanceBetweenAgents <= attackDistance)
        {
            state = EnemyStates.ATTACKING;
        }
    }



    void Attack()
    {
        hitCooldown -= Time.deltaTime;
        if (hitCooldown <= 0)
        {
            HitAgent();
        }
        // Check if the agent is within viewing distance of the enemy
        float distanceBetweenAgents = Vector3.Distance(agent.transform.position, gameObject.transform.position);
        if (distanceBetweenAgents >= attackDistance)
        {
            state = EnemyStates.SEEKING;
        }

        if (hitsRemaining == 0)
        {
            GetComponent<Animator>().enabled = false;
            state = EnemyStates.DEAD;
        }
    }



    void HitAgent()
    {
        // Remove weapon durability from agent
        hitCooldown = 1.0f;
        StartCoroutine(FlashAgentRed());
    }



    IEnumerator FlashAgentRed()
    {
        agent.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        agent.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
