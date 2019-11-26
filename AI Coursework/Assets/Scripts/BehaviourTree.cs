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
    public bool fighting;
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
        State nodeState = State.RUNNING;

        if (!chosenRoom.isExitRoom)
        {
            // Find number of doors in this room
            for (int i = 0; i < chosenRoom.exits.Count; i++)
            {
                if (!chosenRoom.exits[i].reached)
                {
                    chosenRoom.exits[i].priority = 0;
                }

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

            Debug.Log(bestExit.gameObject.name);

            if (WalkToDoor(bestExit) == State.SUCCESS)
            {
                if (OpenDoor(bestExit) == State.SUCCESS)
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



    State OpenDoor(Exit exit)
    {
        State nodeState = State.RUNNING;
        if (exit.isBlocked)
        {
            exit.priority -= 10;
        }
        else nodeState = State.SUCCESS;

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
}
