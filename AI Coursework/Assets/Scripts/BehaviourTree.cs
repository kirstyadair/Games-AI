﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    FAILED, SUCCESS, RUNNING
}

public class BehaviourTree : MonoBehaviour
{
    public bool onPath;
    public bool enemiesClose;
    public bool fighting;
    //public bool doorReached;
    public Room chosenRoom;
    public EnemyAgentScript selectedEnemy;
    AgentScript agent;
    public List<EnemyAgentScript> enemies;
    

    void Start()
    {
        agent = GetComponent<AgentScript>();
        chosenRoom = agent.currentRoom;
        enemies = chosenRoom.enemies;
    }

    // Go through the behaviour tree each frame
    void Update()
    {
        chosenRoom = agent.currentRoom;
        StartNode();
    }

    void StartNode()
    {
        // Find if there are enemies or not
        if (FindEnemies() > 0)
        {
            selectedEnemy = enemies[0];
            
            // Are the enemies close enough to attack?
            // If yes:
            if (AttackEnemies() == State.SUCCESS)
            {
                if (!fighting)
                {
                    fighting = true;
                    FightEnemies();
                }
            }
            // If no:
            else
            {
                SeekEnemies();
            }
        }
        else
        {
            FindPath();
        }
    }

    int FindEnemies()
    {
        int count = chosenRoom.enemies.Count;
        return count;
    }

    State AttackEnemies()
    {
        State nodeState = new State();
        if (Vector3.Distance(selectedEnemy.transform.position, transform.position) < 0.1f)
        {
            nodeState = State.SUCCESS;
        } 
        else
        {
            nodeState = State.FAILED;
        }
        return nodeState;
    }

/*
    // Start here
    void StartNode()
    {
       // If there are enemies around and the agent can seek them
        if (FindNearbyEnemies() == true && SeekEnemy() != State.RUNNING)
        {
            // If not already fighting
            if (!fighting)
            {
                // Fight enemies until they are all dead
                FightEnemies();
            }
            
        }
        // There are no enemies or they are all dead
        else
        {
            // If there is a path through this room
            if (FindExitsInRoom() == 1)
            {
                if (WalkToDoor(chosenRoom.exits[0]) == State.SUCCESS)
                {
                    TryDoor(chosenRoom.exits[0]);
                }
            }
            else if (FindExitsInRoom() > 1)
            {
                WalkToDoor(FindFastestRoute());
            }
            else
            {
                // Return to the previous room, mark door as a failure

            }
        }
    }



    State OnPath()
    {
        State nodeState = new State();
        // If on a path, return RUNNING
        if (onPath)
        {
            nodeState = State.RUNNING;
        }
        // Else return SUCCESS, i.e. the spider has finished using a path and is in a room
        else
        {
            nodeState = State.SUCCESS;
        }
        
        return nodeState;
    }
*/


    State FightEnemies()
    {
        fighting = true;
        State nodeState = new State();
        nodeState = State.RUNNING;
        agent.Fight();
        return nodeState;
    }



    State SeekEnemies()
    {
        State nodeState = new State();
        if (selectedEnemy != null)
        {
            agent.Seek(selectedEnemy.gameObject.transform.position, ref nodeState);
        }
        else
        {
            nodeState = State.SUCCESS;
        }
        
        return nodeState;
    }



    void FindPath()
    {
        if (FindPathThroughThisRoom() != State.FAILED)
        {

        }
    }



    State FindPathThroughThisRoom()
    {
        State nodeState = new State();

        // Find number of doors in this room
        for (int i = 0; i < chosenRoom.exits.Count; i++)
        {
            chosenRoom.exits[i].priority = 0;

            if (chosenRoom.exits[i].type == ExitType.DOOR)
            {
                chosenRoom.exits[i].priority++;
            }
            chosenRoom.exits[i].priority -= (int)chosenRoom.exits[i].roomsToExitRoom;
        }

        Exit bestExit = chosenRoom.exits[0];

        for (int i = 1; i < chosenRoom.exits.Count; i++)
        {
            if (chosenRoom.exits[i].priority > bestExit.priority) bestExit = chosenRoom.exits[i];
        }

        if (WalkToDoor(bestExit) == State.SUCCESS)
        {
            /*if (OpenDoor(bestExit) == State.SUCCESS)
            {
                if (GoThroughDoor(bestExit) == State.SUCCESS)
                {
                    nodeState = State.SUCCESS;
                }
                else if (GoThroughDoor(bestExit) == State.FAILED)
                {
                    nodeState = State.FAILED;
                }
            }
            else if (OpenDoor(bestExit) == State.FAILED)
            {
                nodeState = State.FAILED;
            }*/
        }
        else if (WalkToDoor(bestExit) == State.FAILED)
        {
            nodeState = State.FAILED;
        }

        return nodeState;
    }



    State WalkToDoor(Exit exit)
    {
        State nodeState = State.RUNNING;

        if (!exit.reached)
        {
            agent.Seek(exit.transform.position, ref nodeState);
            if (nodeState == State.SUCCESS)
            {
                exit.reached = true;
            }
        }
        
        return nodeState;
    }


/*
    bool FindNearbyEnemies()
    {
        if (chosenRoom.numberOfEnemies > 0) return true;
        else return false;
    }



    int FindExitsInRoom()
    {
        int exitCount = chosenRoom.exits.Count;

        for (int i = 0; i < chosenRoom.exits.Count; i++)
        {
            if (!chosenRoom.exits[i].isViableExit)
            {
                exitCount--;
            }
        }
        return exitCount;
    }



    State WalkToDoor(Exit exit)
    {
        //if (!doorReached)
        //{
            State nodeState = new State();
            agent.MoveToDoor(exit, ref nodeState);
            return nodeState;
        //}
        //else return State.SUCCESS;
    }



    State TryDoor(Exit exit)
    {
        State nodeState = new State();
        if (exit.isBlocked) nodeState = State.FAILED;
        else 
        {
            agent.Seek(exit.roomForwards.gameObject.GetComponent<Collider2D>().bounds.center, ref nodeState);
            if (nodeState == State.SUCCESS)
            {
                chosenRoom = agent.currentRoom;
            }
        }
        return nodeState;
    }



    Exit FindFastestRoute()
    {
        //State nodeState = State.RUNNING;
        Exit currentFastest = agent.currentRoom.exits[0];
        for (int i = 1; i < agent.currentRoom.exits.Count; i++)
        {
            if (agent.currentRoom.exits[i].roomsToExitRoom < currentFastest.roomsToExitRoom)
            {
                currentFastest = agent.currentRoom.exits[i];
            }
        }
        
        //agent.Seek(currentFastest.transform.position, ref nodeState);
        return currentFastest;
    }
    */
}
