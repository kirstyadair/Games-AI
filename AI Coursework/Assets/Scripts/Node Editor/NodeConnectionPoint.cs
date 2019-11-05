using System;
using UnityEngine;
 
public enum ConnectionPointType 
{ 
    IN, OUT
}
 
public class NodeConnectionPoint
{
    public Rect rectangle;
    public ConnectionPointType cpType;
    public Node theNode;
    public GUIStyle guiStyle;
    public Action<NodeConnectionPoint> ConnectionPoint;
    
    public NodeConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<NodeConnectionPoint> OnClickConnectionPoint)
    {
        this.theNode = node;
        this.cpType = type;
        this.guiStyle = style;
        this.ConnectionPoint = OnClickConnectionPoint;
        rectangle = new Rect(0, 0, 10f, 20f);
    }
 
    public void Draw()
    {
        rectangle.y = theNode.rect.y + (theNode.rect.height * 0.5f) - rectangle.height * 0.5f;
 
        switch (cpType)
        {
            case ConnectionPointType.IN:
            {
                rectangle.x = theNode.rect.x - rectangle.width + 8f;
                break;
            }
 
            case ConnectionPointType.OUT:
            {
                rectangle.x = theNode.rect.x + theNode.rect.width - 8f;
                break;
            }
        }
        
        if (GUI.Button(rectangle, "", guiStyle))
        {
            if (ConnectionPoint != null)
            {
                ConnectionPoint(this);
            }
        }
    }
}