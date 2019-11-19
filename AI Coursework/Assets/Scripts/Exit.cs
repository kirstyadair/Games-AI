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
    public float roomsToExitRoom;
    public bool isViableExit;
    public ExitType type;
    public Room roomForwards;
    public Room roomBack;
}
