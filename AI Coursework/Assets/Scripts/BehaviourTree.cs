using System.Collections;
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
    public bool doorReached;
    int exitAttempted = 0;
    public Room chosenRoom;
    AgentScript agent;
    

    void Start()
    {
        agent = GetComponent<AgentScript>();
        chosenRoom = agent.currentRoom;
    }

    // Go through the behaviour tree each frame
    void Update()
    {
        StartNode();
    }


    // Start here
    void StartNode()
    {
        // If not on a path
        //if (OnPath() != State.RUNNING)
        //{
            // If there are enemies around and the agent can seek them
            if (FindNearbyEnemies() == true && SeekEnemy() != State.RUNNING && OnPath() != State.RUNNING)
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
            
        //}
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



    State FightEnemies()
    {
        fighting = true;
        State nodeState = new State();
        nodeState = State.RUNNING;
        agent.Fight();
        return nodeState;
    }



    State SeekEnemy()
    {
        State nodeState = new State();
        if (agent.currentAttackingEnemies.Count > 0)
        {
            agent.Seek(agent.currentAttackingEnemies[0].gameObject.transform.position, ref nodeState);
        }
        else
        {
            nodeState = State.SUCCESS;
        }
        
        return nodeState;
    }



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
        if (!doorReached)
        {
            State nodeState = new State();
            agent.MoveToDoor(exit, ref nodeState);
            if (nodeState == State.SUCCESS)
            {
                doorReached = true;
            }
            else
            {
                doorReached = false;
            }
            return nodeState;
        }
        else return State.SUCCESS;
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
        Exit currentFastest = agent.currentRoom.exits[0];
        for (int i = 1; i < agent.currentRoom.exits.Count; i++)
        {
            if (agent.currentRoom.exits[i].roomsToExitRoom < currentFastest.roomsToExitRoom)
            {
                currentFastest = agent.currentRoom.exits[i];
            }
        }
        return currentFastest;
    }
}
