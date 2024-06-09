#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

public class PathfindingTest : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Pathfinding pathfinding;
    public GridMap gridMap;
    public float nodeSize = 0.3f; // Adjust this value to make the nodes thicker
    public float lineThickness = 5f; // Adjust the line thickness here

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            List<Node> path = pathfinding.FindPath(startPoint.position, endPoint.position);
            gridMap.path = path;
            if (path != null)
            {
                Debug.Log("Path found with " + path.Count + " nodes.");
            }
            else
            {
                Debug.LogWarning("No path found.");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (gridMap != null && gridMap.path != null)
        {
            for (int i = 0; i < gridMap.path.Count; i++)
            {
                Node currentNode = gridMap.path[i];
                Gizmos.color = Color.green;

                // Draw a thicker sphere at each node
                Gizmos.DrawSphere(currentNode.worldPosition, nodeSize);

                // Draw a line to the next node
                if (i < gridMap.path.Count - 1)
                {
                    Node nextNode = gridMap.path[i + 1];
#if UNITY_EDITOR
                    Handles.color = Color.green;
                    Handles.DrawAAPolyLine(lineThickness, currentNode.worldPosition, nextNode.worldPosition);
#else
                    Gizmos.DrawLine(currentNode.worldPosition, nextNode.worldPosition);
#endif
                }
            }
        }
    }
}
