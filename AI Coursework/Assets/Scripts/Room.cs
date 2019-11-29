using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool visited;
    public bool isExitRoom;
    public int roomsAwayFromExit;
    public List<Room> connectedRooms;
    public Exit[] exits;
    public List<EnemyAgentScript> enemies;

    void Start()
    {
        exits = GetComponentsInChildren<Exit>();
    }
}
