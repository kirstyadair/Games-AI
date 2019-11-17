using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public Room startRoom;
    public List<EnemyAgentScript> currentAttackingEnemies;
    public Room currentRoom;

    Animator animator;
    BehaviourTree behaviourTree;
    Room nextRoom;
    Vector3 desiredVelocity;



    void Start()
    {
        currentRoom = startRoom;
        currentRoom.visited = true;
        behaviourTree = GetComponent<BehaviourTree>();
        animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * 0.1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * 0.1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 0.1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * 0.1f;
        }
        nextRoom = FindNextRoom();
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Room")
        {
            currentRoom = other.GetComponent<Room>();
            currentRoom.visited = true;
        }
        if (other.tag == "Path")
        {
            behaviourTree.onPath = true;
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Path")
        {
            behaviourTree.onPath = false;
        }
    }



    public void Fight()
    {
        StartCoroutine(FightEnemy());
    }



    Room FindNextRoom()
    {
        Room bestRoom;
        List<Room> rooms = new List<Room>();
        rooms = currentRoom.connectedRooms;

        bestRoom = rooms[0];

        
        for (int i = 1; i < rooms.Count; i++)
        {
            if (rooms[i].isExitRoom) bestRoom = rooms[i];
            else 
            {
                if (rooms[i].visited) continue;
                else if (rooms[i].numberOfEnemies <= bestRoom.numberOfEnemies) bestRoom = rooms[i];
            }
            
        }

        return bestRoom;
    }



    public void Seek(Vector3 targetPosition, State nodeState)
    {
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * 0.05f;
        transform.position += desiredVelocity;
        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            nodeState = State.SUCCESS;
        }
        
    }


    IEnumerator FightEnemy()
    {
        if (currentAttackingEnemies.Count > 0)
        {
            EnemyAgentScript currentEnemy = currentAttackingEnemies[0];

            // while current enemy has health
            while (currentEnemy.hitsRemaining > 0)
            {
                Debug.Log("hit");
                currentEnemy.hitsRemaining--;
                yield return new WaitForSeconds(1.0f);
                
            }

            currentAttackingEnemies.Remove(currentEnemy);
        
            // Once all enemies killed
            Debug.Log("all killed");
            
        }
        behaviourTree.fighting = false;
    }
}
