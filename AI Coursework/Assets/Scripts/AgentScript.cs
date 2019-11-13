using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public Room startRoom;

    Room currentRoom;
    Room nextRoom;
    Vector3 targetPosition;
    Vector3 desiredVelocity;

    void Start()
    {
        currentRoom = startRoom;
        currentRoom.visited = true;
    }

    // Update is called once per frame
    void Update()
    {
        nextRoom = FindNextRoom();
        targetPosition = nextRoom.transform.position;
        SeekNextRoom();
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Room")
        {
            currentRoom = other.GetComponent<Room>();
            currentRoom.visited = true;
        }
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
            else if (rooms[i].visited) continue;
            else if (rooms[i].numberOfEnemies <= bestRoom.numberOfEnemies) bestRoom = rooms[i];
            
        }

        return bestRoom;
    }



    void SeekNextRoom()
    {
		desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * 0.05f;
        transform.position += desiredVelocity;
    }
}
