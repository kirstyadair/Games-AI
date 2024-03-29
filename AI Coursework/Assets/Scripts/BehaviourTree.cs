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
    public bool fighting;
    bool goingBack;
    public Room chosenRoom;
    Exit bestExit;
    public EnemyAgentScript selectedEnemy;
    AgentScript agent;
    public List<EnemyAgentScript> enemies;
    

    void Start()
    {
        agent = GetComponent<AgentScript>();
        chosenRoom = agent.currentRoom;
    }



    // Go through the behaviour tree each frame
    void Update()
    {
        enemies = chosenRoom.enemies;
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
        if (FindPathThroughThisRoom() == State.FAILED)
        {
            goingBack = true;
            if (ReturnToPreviousRoom() == State.FAILED)
            {
                Debug.Log("Tree failed");
            }
        }
    }



    State FindPathThroughThisRoom()
    {
        State nodeState = State.RUNNING;
        if (!goingBack)
        {
            if (!chosenRoom.isExitRoom)
            {
                FindExits();

                if (WalkToDoor(bestExit) == State.SUCCESS)
                {
                    // If the exit with the highest priority is blocked
                    if (bestExit.isBlocked) 
                    {
                        // If the agent has not already tried to break the exit
                        if (!bestExit.tried)
                        {
                            // If TryDoor returns false
                            if (!TryDoor())
                            {
                                // Return failure
                                nodeState = State.FAILED;
                                return nodeState;
                            }
                            else
                            {
                                // If successful, set isBlocked to false
                                bestExit.isBlocked = false;
                            }
                            bestExit.tried = true;
                        }
                        else
                        {
                            return nodeState = State.FAILED;
                        }
                    }
                    if (GoThroughDoor(bestExit) == State.SUCCESS)
                    {
                        nodeState = State.SUCCESS;
                    }
                    else if (GoThroughDoor(bestExit) == State.FAILED)
                    {
                        nodeState = State.FAILED;
                    }
                }
                else if (WalkToDoor(bestExit) == State.FAILED)
                {
                    nodeState = State.FAILED;
                }
            }
            else
            {
                agent.Seek(chosenRoom.transform.position, ref nodeState);
                if (nodeState == State.SUCCESS)
                {
                    agent.speed = 0;
                }
            }
        }
        else
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
        else
        {
            nodeState = State.SUCCESS;
        }
        
        return nodeState;
    }



    State GoThroughDoor(Exit exit)
    {
        State nodeState = State.RUNNING;
        agent.Seek(exit.roomForwards.transform.position, ref nodeState);

        if (nodeState == State.SUCCESS)
        {

        }

        return nodeState;
    }



    State ReturnToPreviousRoom()
    {
        State nodeState = State.RUNNING;

        // Seek the center of the room connected to the last used door
        agent.Seek(agent.lastUsedDoor.roomBack.transform.position, ref nodeState);

        if (nodeState == State.SUCCESS)
        {
            // Once in the room, find another door to exit through
            AttemptAnotherDoor();
        }

        return nodeState;
    }



    void AttemptAnotherDoor()
    {
        agent.lastUsedDoor.isViableExit = false;
        FindExits();
        goingBack = false;
    }


    
    void FindExits()
    {
        // Find number of doors in this room
        for (int i = 0; i < chosenRoom.exits.Length; i++)
        {
            chosenRoom.exits[i].priority = 0;

            // Check if the exit is viable
            if (chosenRoom.exits[i].isViableExit)
            {
                // Add 1 to priority if the exit is a door
                if (chosenRoom.exits[i].type == ExitType.DOOR) chosenRoom.exits[i].priority++;
                // Minus 1 if the exit is a window
                if (chosenRoom.exits[i].type == ExitType.WINDOW) chosenRoom.exits[i].priority--;
                // If the exit is blocked, -5 to priority
                if (chosenRoom.exits[i].isBlocked) chosenRoom.exits[i].priority -= 5;
                // If the exit has been visited already, -10 to priority.  This prevents oscillation
                if (chosenRoom.exits[i].exitVisited) chosenRoom.exits[i].priority -= 10;
                // Subtract the number of enemies from priority
                chosenRoom.exits[i].priority -= chosenRoom.exits[i].roomForwards.enemies.Count;
            }
            else
            {
                // If the exit is not viable, -100 to priority
                chosenRoom.exits[i].priority -= 100;
            }
            
            // Subtract the number of remaining rooms from priority
            chosenRoom.exits[i].priority -= (int)chosenRoom.exits[i].roomsToExitRoom;
        }

        // With the priority worked out, check against all other exit priorities and find the highest
        bestExit = chosenRoom.exits[0];

        for (int i = 1; i < chosenRoom.exits.Length; i++)
        {
            if (chosenRoom.exits[i].priority > bestExit.priority) bestExit = chosenRoom.exits[i];
        }
    }



    bool TryDoor()
    {
        // Randomly decide whether this was successful or unsuccessful
        return (Random.value > 0.5f);
    }
}
