using System.Collections.Generic;
using UnityEngine;

public class HealthPotionGenerator : MonoBehaviour
{
    [SerializeField] private GameObject healthPotionPrefab; // Prefab of the health potion
    [SerializeField] private int healthPotionsPerRoom = 2; // Number of health potions to generate per room
    private List<GameObject> healthPotions = new List<GameObject>(); // List to keep track of generated health potions

    public void GenerateHealthPotions(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        ClearHealthPotions();
        PlaceNewHealthPotions(floor, roomsList);
    }

    private void ClearHealthPotions()
    {
        foreach (GameObject potion in healthPotions)
        {
            Destroy(potion);
        }
        healthPotions.Clear();
    }

    private void PlaceNewHealthPotions(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        foreach (var room in roomsList)
        {
            List<Vector2Int> roomFloorPositions = new List<Vector2Int>();

            // Collect all valid floor positions within the room
            for (int col = room.xMin + 1; col < room.xMax - 1; col++)
            {
                for (int row = room.yMin + 1; row < room.yMax - 1; row++)
                {
                    Vector2Int position = new Vector2Int(col, row);
                    if (floor.Contains(position) && !IsOccupied(position) && IsInsideRoom(position, room))
                    {
                        roomFloorPositions.Add(position);
                    }
                }
            }

            // Place the specified number of health potions in this room
            int potionsPlacedInRoom = 0;
            while (potionsPlacedInRoom < healthPotionsPerRoom && roomFloorPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, roomFloorPositions.Count);
                Vector2Int potionPosition = roomFloorPositions[randomIndex];
                roomFloorPositions.RemoveAt(randomIndex);

                GameObject potion = Instantiate(healthPotionPrefab, new Vector3(potionPosition.x, potionPosition.y, 0), Quaternion.identity);
                healthPotions.Add(potion);
                potionsPlacedInRoom++;
            }
        }
    }

    private bool IsInsideRoom(Vector2Int position, BoundsInt room)
    {
        // Check if the position is inside the given room bounds
        return position.x > room.xMin && position.x < room.xMax && position.y > room.yMin && position.y < room.yMax;
    }

    private bool IsOccupied(Vector2Int position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(position.x, position.y));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy") || collider.CompareTag("HealthPotion") || collider.CompareTag("Coin") || collider.CompareTag("Boss"))
            {
                return true;
            }
        }
        return false;
    }
}
