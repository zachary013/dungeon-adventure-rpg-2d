using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public bool walkable;
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector2Int _gridPosition, Vector3 _worldPosition)
    {
        walkable = _walkable;
        gridPosition = _gridPosition;
        worldPosition = _worldPosition;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return compare;
    }
}
