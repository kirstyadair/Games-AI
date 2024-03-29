﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public Room startRoom;
    public List<EnemyAgentScript> currentAttackingEnemies;
    public Room currentRoom;
    public Room nextRoom;
    public Exit lastUsedDoor;
    public float speed;

    Animator animator;
    BehaviourTree behaviourTree;
    Vector3 desiredVelocity;



    void Start()
    {
        currentRoom = startRoom;
        currentRoom.visited = true;
        behaviourTree = GetComponent<BehaviourTree>();
        animator = GetComponent<Animator>();
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
        if (other.tag == "Exit")
        {
            lastUsedDoor = other.GetComponent<Exit>();
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Path")
        {
            behaviourTree.onPath = false;
        }
        if (other.tag == "Exit")
        {
            StartCoroutine(MarkDoor(other));
            
        }
    }



    IEnumerator MarkDoor(Collider2D exit)
    {
        yield return new WaitForSeconds(1);
        exit.GetComponent<Exit>().exitVisited = true;
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
                else if (rooms[i].enemies.Count <= bestRoom.enemies.Count) bestRoom = rooms[i];
            }
            
        }

        return bestRoom;
    }



    public void Seek(Vector3 targetPosition, ref State nodeState)
    {
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * speed;
        transform.position += desiredVelocity;
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            nodeState = State.SUCCESS;
        }
        else
        {
            nodeState = State.RUNNING;
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
                currentEnemy.hitsRemaining--;
                yield return new WaitForSeconds(1.0f);
                
            }

            currentAttackingEnemies.Remove(currentEnemy);
        
            // Once all enemies killed
            Debug.Log("all killed");
            behaviourTree.fighting = false;
        }
        
    }



    public void MoveToDoor(Exit exit, ref State nodeState)
    {
        if (exit.isViableExit)
        {
            Seek(exit.transform.position, ref nodeState);
            
        }
    }
}
