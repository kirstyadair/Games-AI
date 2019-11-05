using System;
using UnityEditor;
using UnityEngine;

public class Node
{
    public Rect rect;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public NodeConnectionPoint inConnection;
    public NodeConnectionPoint outConnection;
    public string name;
    public bool dragged;
    public bool selected;
    public Action<Node> RemoveNode;


    public Node(float width, float height, Vector2 pos, GUIStyle style, GUIStyle selectedStyle, GUIStyle inConnectionStyle, GUIStyle outConnectionStyle, Action<NodeConnectionPoint> clickInPoint, Action<NodeConnectionPoint> clickOutPoint, Action<Node> clickRemoveNode)
    {
        rect = new Rect(pos.x, pos.y, width, height);
        this.style = style;
        inConnection = new NodeConnectionPoint(this, ConnectionPointType.IN, inConnectionStyle, clickInPoint);
        outConnection = new NodeConnectionPoint(this, ConnectionPointType.OUT, outConnectionStyle, clickOutPoint);
        defaultNodeStyle = style;
        selectedNodeStyle = selectedStyle;
        RemoveNode = clickRemoveNode;
    }


    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }


    public void DrawNode()
    {
        inConnection.Draw();
        outConnection.Draw();
        GUI.Box(rect, name, style);
    }


    public bool ProcessEvents(Event theEvent)
    {
        switch (theEvent.type)
        {
            case EventType.MouseDown:
            {
                if (theEvent.button == 0)
                {
                    if (rect.Contains(theEvent.mousePosition))
                    {
                        dragged = true;
                        GUI.changed = true;
                        selected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        selected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (theEvent.button == 1 && selected && rect.Contains(theEvent.mousePosition))
                {
                    ProcessContextMenu();
                    theEvent.Use();
                }
                break;
            }
 
            case EventType.MouseUp:
            {
                dragged = false;
                break;
            }
 
            case EventType.MouseDrag:
            {
                if (theEvent.button == 0 && dragged)
                {
                    Drag(theEvent.delta);
                    theEvent.Use();
                    return true;
                }
                break;
            }
        }
 
        return false;
    }


    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
 

    void OnClickRemoveNode()
    {
        if (RemoveNode != null)
        {
            RemoveNode(this);
        }
    }
}
