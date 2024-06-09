using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    public int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField] public Transform playerPrefab;
    [SerializeField] private CoinGenerator coinGenerator;
    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private HealthPotionGenerator potionGenerator;
    [SerializeField] private PropsGenerator propsGenerator;
    [SerializeField] public Transform bossPrefab;

    public List<BoundsInt> roomOrigins;
    private HashSet<Vector2Int> floor;

    public void playRunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration();
        GenerateCoins();
        GenerateEnemies();
        GenerateProps();
        GenerateHealthPotions();
    }

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        roomOrigins = roomsList;
        floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        playerPrefab.position = roomsList[0].center;
        PlaceBossInFarthestRoom();
    }

    private void GenerateCoins()
    {
        coinGenerator.GenerateCoins(floor, roomOrigins);
    }

    private void GenerateEnemies()
    {
        enemyGenerator.GenerateEnemies(floor, roomOrigins);
    }

    private void GenerateProps() // Add this method
    {
        propsGenerator.GenerateProps(floor, roomOrigins);
    }

    private void GenerateHealthPotions()
    {
        potionGenerator.GenerateHealthPotions(floor, roomOrigins); // Call the method to generate health potions
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private void PlaceBossInFarthestRoom()
    {
        if (bossPrefab != null && roomOrigins != null && roomOrigins.Count > 1)
        {
            // Get the room containing the player
            BoundsInt playerRoom = GetContainingRoom((Vector2Int)Vector3Int.RoundToInt(playerPrefab.position));

            // Initialize variables to track the farthest room and its distance from the player
            BoundsInt farthestRoom = roomOrigins[0];
            float maxDistance = 0;

            // Iterate through all rooms except the player's room
            foreach (var room in roomOrigins)
            {
                if (room != playerRoom)
                {
                    // Calculate the distance between the player's room and the current room
                    float distance = CalculateDistance((Vector2Int)Vector3Int.RoundToInt(playerRoom.center), (Vector2Int)Vector3Int.RoundToInt(room.center));

                    // Update the farthest room if the current room is farther
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestRoom = room;
                    }
                }
            }

            // Place the boss at the center of the farthest room
            Vector3 bossPosition = farthestRoom.center;
            bossPrefab.position = bossPosition;
        }
    }

    private float CalculateDistance(Vector2Int pointA, Vector2Int pointB)
    {
        return Vector2.Distance(pointA, pointB);
    }

    private BoundsInt GetContainingRoom(Vector2Int position)
    {
        foreach (var room in roomOrigins)
        {
            if (room.Contains((Vector3Int)position))
            {
                return room;
            }
        }
        return new BoundsInt();
    }


    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; col < room.size.y - offset; col++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
