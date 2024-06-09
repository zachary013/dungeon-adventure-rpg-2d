using UnityEngine;
using System.Collections.Generic;

public class PropsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] propPrefabs;
    [SerializeField] private int propsPerRoom = 5;
    [SerializeField] private float minDistanceBetweenProps = 1.5f; // Minimum distance between props
    private List<GameObject> props = new List<GameObject>();
    public GridMap gridMap; // Reference to the GridMap script

    public void GenerateProps(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        ClearProps();
        PlaceNewProps(floor, roomsList);
    }

    private void ClearProps()
    {
        foreach (GameObject prop in props)
        {
            Destroy(prop);
        }
        props.Clear();
    }

    private void PlaceNewProps(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        List<Vector2Int> propPositions = new List<Vector2Int>(); // List to keep track of prop positions

        foreach (var room in roomsList)
        {
            List<Vector2Int> roomFloorPositions = new List<Vector2Int>();

            for (int col = room.xMin + 1; col < room.xMax - 1; col++)
            {
                for (int row = room.yMin + 1; row < room.yMax - 1; row++)
                {
                    Vector2Int position = new Vector2Int(col, row);
                    if (floor.Contains(position) && !IsOccupied(position) && !IsNearWall(position, room) && !IsTooCloseToOtherProps(position, propPositions))
                    {
                        roomFloorPositions.Add(position);
                    }
                }
            }

            int propsPlacedInRoom = 0;
            while (propsPlacedInRoom < propsPerRoom && roomFloorPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, roomFloorPositions.Count);
                Vector2Int propPosition = roomFloorPositions[randomIndex];
                roomFloorPositions.RemoveAt(randomIndex);

                GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Length)];
                GameObject prop = Instantiate(propPrefab, new Vector3(propPosition.x, propPosition.y, 0), Quaternion.identity);
                props.Add(prop);
                propPositions.Add(propPosition); // Add position to the list
                propsPlacedInRoom++;
            }
        }

        // Notify the grid map about the prop positions
        gridMap.MarkPropsAsObstacles(propPositions);
    }

    private bool IsOccupied(Vector2Int position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(position.x, position.y));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy") || collider.CompareTag("Prop") || collider.CompareTag("Coin") || collider.CompareTag("Boss"))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsNearWall(Vector2Int position, BoundsInt room)
    {
        return (position.x <= room.xMin + 1 || position.x >= room.xMax - 2 || position.y <= room.yMin + 1 || position.y >= room.yMax - 2);
    }

    private bool IsTooCloseToOtherProps(Vector2Int position, List<Vector2Int> propPositions)
    {
        foreach (var propPos in propPositions)
        {
            if (Vector2.Distance(position, propPos) < minDistanceBetweenProps)
            {
                return true;
            }
        }
        return false;
    }
}
