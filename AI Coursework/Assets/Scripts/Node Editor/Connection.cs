using System;
using UnityEditor;
using UnityEngine;
 
public class Connection
{
    public NodeConnectionPoint connectIn;
    public NodeConnectionPoint connectOut;
    public Action<Connection> RemoveConnection;
 
    public Connection(NodeConnectionPoint connectIn, NodeConnectionPoint connectOut, Action<Connection> RemoveConnection)
    {
        this.connectIn = connectIn;
        this.connectOut = connectOut;
        this.RemoveConnection = RemoveConnection;
    }
 
    public void Draw()
    {
        Handles.DrawBezier(connectIn.rectangle.center, connectOut.rectangle.center, connectIn.rectangle.center + Vector2.left * 50f, 
                            connectOut.rectangle.center - Vector2.left * 50f, Color.white, null, 2f);
        if (Handles.Button((connectIn.rectangle.center + connectOut.rectangle.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (RemoveConnection != null)
            {
                RemoveConnection(this);
            }
        }
    }
}