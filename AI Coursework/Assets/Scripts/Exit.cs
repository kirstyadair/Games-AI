using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExitType
{
    DOOR, WINDOW
}

public class Exit : MonoBehaviour
{
    public bool isBlocked;
    public bool reached;
    public float roomsToExitRoom;
    public bool isViableExit;
    public int priority;
    public ExitType type;
    public Room roomForwards;
    public Room roomBack;
}
