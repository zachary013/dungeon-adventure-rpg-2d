using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab1;
    [SerializeField] private GameObject enemyPrefab2;
    [SerializeField] private GameObject enemyPrefab3;
    [SerializeField] private int enemiesPerRoom = 3; // Number of enemies to generate per room
    private List<GameObject> enemies = new List<GameObject>();

    public void GenerateEnemies(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        ClearEnemies();
        PlaceNewEnemies(floor, roomsList);
    }

    private void ClearEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
    }

    private void PlaceNewEnemies(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        foreach (var room in roomsList)
        {
            List<Vector2Int> roomFloorPositions = new List<Vector2Int>();

            // Collect all valid floor positions within the room
            for (int col = room.xMin + 1; col < room.xMax; col++)
            {
                for (int row = room.yMin + 1; row < room.yMax; row++)
                {
                    Vector2Int position = new Vector2Int(col, row);
                    if (floor.Contains(position) && !IsOccupied(position, "Player") && !IsOccupied(position, "Coin") && !IsOccupied(position, "Boss"))
                    {
                        roomFloorPositions.Add(position);
                    }
                }
            }

            // Ensure we place all types of enemies in each room
            List<GameObject> enemyPrefabs = new List<GameObject> { enemyPrefab1, enemyPrefab2, enemyPrefab3 };
            for (int i = 0; i < enemiesPerRoom && roomFloorPositions.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, roomFloorPositions.Count);
                Vector2Int enemyPosition = roomFloorPositions[randomIndex];
                roomFloorPositions.RemoveAt(randomIndex);

                if (floor.Contains(enemyPosition)) // Ensure the position is within the floor
                {
                    int prefabIndex = i % enemyPrefabs.Count;
                    GameObject enemyPrefab = enemyPrefabs[prefabIndex];
                    GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyPosition.x, enemyPosition.y, 0), Quaternion.identity);
                    enemies.Add(enemy);

                    // Debug log to check which enemies are being instantiated
                    Debug.Log($"Generated enemy of type {enemyPrefab.name} at position {enemyPosition}");
                }
                else
                {
                    Debug.LogWarning($"Tried to generate enemy at invalid position {enemyPosition}");
                }
            }

            // If there are still positions left but all enemy types have been placed, place additional random enemies
            while (roomFloorPositions.Count > 0 && enemies.Count < enemiesPerRoom)
            {
                int randomIndex = Random.Range(0, roomFloorPositions.Count);
                Vector2Int enemyPosition = roomFloorPositions[randomIndex];
                roomFloorPositions.RemoveAt(randomIndex);

                if (floor.Contains(enemyPosition)) // Ensure the position is within the floor
                {
                    GameObject enemyPrefab = GetRandomEnemyPrefab();
                    GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyPosition.x, enemyPosition.y, 0), Quaternion.identity);
                    enemies.Add(enemy);

                    // Debug log to check which enemies are being instantiated
                    Debug.Log($"Generated random enemy of type {enemyPrefab.name} at position {enemyPosition}");
                }
                else
                {
                    Debug.LogWarning($"Tried to generate enemy at invalid position {enemyPosition}");
                }
            }
        }
    }

    private bool IsOccupied(Vector2Int position, string tag)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(position.x, position.y));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int randomIndex = Random.Range(0, 3); // Choose a random index between 0 and 2 (inclusive)
        switch (randomIndex)
        {
            case 0:
                return enemyPrefab1;
            case 1:
                return enemyPrefab2;
            default:
                return enemyPrefab3;
        }
    }
} 
