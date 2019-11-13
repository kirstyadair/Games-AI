using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    FAILED, SUCCESS, RUNNING
}

public class BehaviourTree : MonoBehaviour
{

    // Go through the behaviour tree each frame
    void Update()
    {
        StartNode();
    }


    // Start here
    void StartNode()
    {
        if (FindRoomDetails() != State.RUNNING)
        {
            if (FindRoomDetails() == State.SUCCESS)
            {
                Debug.Log("find room info success");
            }
            else
            {
                Debug.Log("find room info fail");
            }
        }
    }

    State FindRoomDetails()
    {
        State nodeState = new State();
        if (FindRoomName() == State.SUCCESS && FindRoomEnemies() == State.SUCCESS && FindRoomExits() == State.SUCCESS)
        {
            nodeState = State.SUCCESS;
        }
        else
        {
            nodeState = State.FAILED;
        }
        return nodeState;
    }

    State FindRoomName()
    {
        State nodeState = new State();
        nodeState = State.SUCCESS;
        return nodeState;
    }

    State FindRoomEnemies()
    {
        State nodeState = new State();
        nodeState = State.SUCCESS;
        return nodeState;
    }

    State FindRoomExits()
    {
        State nodeState = new State();
        nodeState = State.SUCCESS;
        return nodeState;
    }
}
