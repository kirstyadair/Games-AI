using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    FAILED, SUCCESS, RUNNING, NULL
}

public class TreeNode
{
    public State state;
}

public class BehaviourTree : MonoBehaviour
{
    public bool onPath;
    public bool enemiesClose;
    public bool fighting;
    AgentScript agent;
    

    void Start()
    {
        agent = GetComponent<AgentScript>();
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
        if (OnPath() != State.RUNNING)
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
                // Find the next room based on how close it is to the exit room

                // C
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
            agent.Seek(agent.currentAttackingEnemies[0].gameObject.transform.position, nodeState);
        }
        else
        {
            nodeState = State.SUCCESS;
        }
        
        return nodeState;
    }



    bool FindNearbyEnemies()
    {
        if (agent.currentRoom.numberOfEnemies > 0) return true;
        else return false;
    }
}
