using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsPerRoom = 5; // Number of coins to generate per room
    private List<GameObject> coins = new List<GameObject>();

    public void GenerateCoins(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
    {
        ClearCoins();
        PlaceNewCoins(floor, roomsList);
    }

    private void ClearCoins()
    {
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
        coins.Clear();
    }

    private void PlaceNewCoins(HashSet<Vector2Int> floor, List<BoundsInt> roomsList)
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
                    if (floor.Contains(position))
                    {
                        roomFloorPositions.Add(position);
                    }
                }
            }

            // Place the specified number of coins in this room
            for (int i = 0; i < coinsPerRoom && roomFloorPositions.Count > 0; i++)
            {
                // Find a valid coin position that does not overlap with players, bosses, or enemies
                Vector2Int coinPosition = FindValidCoinPosition(roomFloorPositions);
                if (coinPosition != Vector2Int.zero)
                {
                    GameObject coin = Instantiate(coinPrefab, new Vector3(coinPosition.x, coinPosition.y, 0), Quaternion.identity);
                    coins.Add(coin);
                    roomFloorPositions.Remove(coinPosition); // Remove the position from available positions
                }
            }
        }
    }

    private Vector2Int FindValidCoinPosition(List<Vector2Int> positions)
    {
        // Shuffle the positions randomly to reduce predictability
        Shuffle(positions);

        // Check each position for overlap with players, bosses, or enemies
        foreach (Vector2Int position in positions)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(position.x, position.y), 0.1f);

            bool overlap = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player") || collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
                {
                    overlap = true;
                    break;
                }
            }

            // Return the position if it does not overlap with players, bosses, or enemies
            if (!overlap)
            {
                return position;
            }
        }

        // Return zero vector if no valid position is found
        return Vector2Int.zero;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }
}
