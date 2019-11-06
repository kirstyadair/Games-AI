using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
    List<Node> nodesList;
    List<Connection> connectionsList;
    GUIStyle style;
    GUIStyle inConnectionStyle;
    GUIStyle outConnectionStyle;
    GUIStyle selectedStyle;
    NodeConnectionPoint selectedInConnection;
    NodeConnectionPoint selectedOutConnection;
    Vector2 canvasDrag;

    
    [MenuItem("Node Editor/Behaviour Tree")]
    static void OpenBehaviourTreeWindow()
    {
        NodeEditor editorWindow = GetWindow<NodeEditor>();
        editorWindow.titleContent = new GUIContent("Behaviour Tree");
    }


    void OnEnable()
    {
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);

        inConnectionStyle = new GUIStyle();
        inConnectionStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inConnectionStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inConnectionStyle.border = new RectOffset(4, 4, 12, 12);
 
        outConnectionStyle = new GUIStyle();
        outConnectionStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outConnectionStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outConnectionStyle.border = new RectOffset(4, 4, 12, 12);

        selectedStyle = new GUIStyle();
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
        selectedStyle.border = new RectOffset(12, 12, 12, 12);
    }


    void OnGUI()
    {
        DrawAllNodes();
        DrawAllConnections();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed)
        {
            Repaint();
        }
    }


    void DrawAllNodes()
    {
        if (nodesList != null)
        {
            for (int i = 0; i < nodesList.Count; i++)
            {
                nodesList[i].DrawNode();
            }
        }
    }


    void ProcessEvents(Event theEvent)
    {
        canvasDrag = Vector2.zero;
        switch (theEvent.type)
        {
            case EventType.MouseDown:
            {
                if (theEvent.button == 1)
                {
                    ProcessContextMenu(theEvent.mousePosition);
                }
                break;
            }
            case EventType.MouseDrag:
            {
                if (theEvent.button == 0)
                {
                    OnCanvasDrag(theEvent.delta);
                }
                break;
            }
        }
    }


    void ProcessNodeEvents(Event theEvent)
    {
        if (nodesList != null)
        {
            for (int i = nodesList.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodesList[i].ProcessEvents(theEvent);
 
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }


    void ProcessContextMenu(Vector2 mousePos)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePos)); 
        menu.ShowAsContext();
    }
 

    void OnClickAddNode(Vector2 mousePos)
    {
        if (nodesList == null)
        {
            nodesList = new List<Node>();
        }
 
        nodesList.Add(new Node(150, 100, mousePos, style, selectedStyle, inConnectionStyle, outConnectionStyle, OnClickInConnection, OnClickOutConnection, OnClickRemoveNode));
    }

    
    void DrawAllConnections()
    {
        if (connectionsList != null)
        {
            for (int i = 0; i < connectionsList.Count; i++)
            {
                connectionsList[i].Draw();
            } 
        }
    }


    void OnClickInConnection(NodeConnectionPoint inConnection)
    {
        selectedInConnection = inConnection;
 
        if (selectedOutConnection != null)
        {
            if (selectedOutConnection.theNode != selectedInConnection.theNode)
            {
                CreateConnection();
                ClearConnectionSelection(); 
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
 

    void OnClickOutConnection(NodeConnectionPoint outConnection)
    {
        selectedOutConnection = outConnection;
 
        if (selectedInConnection != null)
        {
            if (selectedOutConnection.theNode != selectedInConnection.theNode)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
 

    void OnClickRemoveConnection(Connection connection)
    {
        connectionsList.Remove(connection);
    }
 

    void CreateConnection()
    {
        if (connectionsList == null)
        {
            connectionsList = new List<Connection>();
        }
 
        connectionsList.Add(new Connection(selectedInConnection, selectedOutConnection, OnClickRemoveConnection));
    }
 

    void ClearConnectionSelection()
    {
        selectedInConnection = null;
        selectedOutConnection = null;
    }


    void OnClickRemoveNode(Node node)
    {
        if (connectionsList != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();
 
            for (int i = 0; i < connectionsList.Count; i++)
            {
                if (connectionsList[i].connectIn == node.inConnection || connectionsList[i].connectOut == node.outConnection)
                {
                    connectionsToRemove.Add(connectionsList[i]);
                }
            }
 
            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connectionsList.Remove(connectionsToRemove[i]);
            }
 
            connectionsToRemove = null;
        }
 
        nodesList.Remove(node);
    }


    void OnCanvasDrag(Vector2 delta)
    {
        canvasDrag = delta;
 
        if (nodesList != null)
        {
            for (int i = 0; i < nodesList.Count; i++)
            {
                nodesList[i].Drag(delta);
            }
        }
 
        GUI.changed = true;
    }
}
