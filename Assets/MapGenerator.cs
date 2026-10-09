using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject wallPrefab;

    [Header("House Settings")]
    public int houseWidth = 36;       // Overall house width
    public int houseHeight = 36;      // Overall house height
    public int minRoomSize = 6;       // Minimum room dimension
    public int doorwaySize = 3;       // Size of doorways between rooms
    public float playerSafeRadius = 4f; // Safe radius around (0,0)

    private bool[,] wallGrid;

    void Start()
    {
        GenerateRandomHouse();
    }

    public void GenerateRandomHouse()
    {
        // 1. Destroy any existing wall objects under GameManager
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (wallPrefab == null) return;

        int halfW = houseWidth / 2;
        int halfH = houseHeight / 2;
        wallGrid = new bool[houseWidth + 1, houseHeight + 1];

        // 2. Build Solid Outer Perimeter Walls
        for (int x = -halfW; x <= halfW; x++)
        {
            SetWall(x, -halfH, halfW, halfH);
            SetWall(x, halfH, halfW, halfH);
        }
        for (int y = -halfH; y <= halfH; y++)
        {
            SetWall(-halfW, y, halfW, halfH);
            SetWall(halfW, y, halfW, halfH);
        }

        // 3. Randomly Partition House into Rooms (BSP Division)
        List<RectInt> rooms = new List<RectInt>();
        RectInt initialHouse = new RectInt(-halfW + 1, -halfH + 1, houseWidth - 1, houseHeight - 1);
        SplitRegion(initialHouse, rooms);

        // 4. Carve Random Doorways along Room Dividers
        CarveRandomDoorways(rooms, halfW, halfH);

        // 5. Clear Center Spawn Area for Player
        ClearCenterSpawn(halfW, halfH);

        // 6. Instantiate Wall Prefabs in Unity Scene
        for (int x = -halfW; x <= halfW; x++)
        {
            for (int y = -halfH; y <= halfH; y++)
            {
                int gx = x + halfW;
                int gy = y + halfH;

                if (wallGrid[gx, gy])
                {
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                }
            }
        }
    }

    void SplitRegion(RectInt region, List<RectInt> roomList)
    {
        // Stop splitting if region is small enough
        if (region.width < minRoomSize * 2 || region.height < minRoomSize * 2)
        {
            roomList.Add(region);
            return;
        }

        bool splitH = Random.value > 0.5f;
        if (region.width > region.height && region.width / (float)region.height > 1.25f) splitH = false;
        else if (region.height > region.width && region.height / (float)region.width > 1.25f) splitH = true;

        if (splitH) // Horizontal split
        {
            int splitY = Random.Range(region.yMin + minRoomSize, region.yMax - minRoomSize);
            for (int x = region.xMin; x < region.xMax; x++)
            {
                SetWall(x, splitY, houseWidth / 2, houseHeight / 2);
            }
            SplitRegion(new RectInt(region.xMin, region.yMin, region.width, splitY - region.yMin), roomList);
            SplitRegion(new RectInt(region.xMin, splitY + 1, region.width, region.yMax - splitY - 1), roomList);
        }
        else // Vertical split
        {
            int splitX = Random.Range(region.xMin + minRoomSize, region.xMax - minRoomSize);
            for (int y = region.yMin; y < region.yMax; y++)
            {
                SetWall(splitX, y, houseWidth / 2, houseHeight / 2);
            }
            SplitRegion(new RectInt(region.xMin, region.yMin, splitX - region.xMin, region.height), roomList);
            SplitRegion(new RectInt(splitX + 1, region.yMin, region.xMax - splitX - 1, region.height), roomList);
        }
    }

    void CarveRandomDoorways(List<RectInt> rooms, int halfW, int halfH)
    {
        int halfDoor = doorwaySize / 2;

        foreach (var room in rooms)
        {
            // Pick a random doorway on horizontal or vertical room boundary
            if (Random.value > 0.5f && room.width > doorwaySize + 2)
            {
                int doorX = Random.Range(room.xMin + 1 + halfDoor, room.xMax - 1 - halfDoor);
                for (int dx = -halfDoor; dx <= halfDoor; dx++)
                {
                    ClearWall(doorX + dx, room.yMin - 1, halfW, halfH);
                    ClearWall(doorX + dx, room.yMax, halfW, halfH);
                }
            }
            else if (room.height > doorwaySize + 2)
            {
                int doorY = Random.Range(room.yMin + 1 + halfDoor, room.yMax - 1 - halfDoor);
                for (int dy = -halfDoor; dy <= halfDoor; dy++)
                {
                    ClearWall(room.xMin - 1, doorY + dy, halfW, halfH);
                    ClearWall(room.xMax, doorY + dy, halfW, halfH);
                }
            }
        }
    }

    void ClearCenterSpawn(int halfW, int halfH)
    {
        for (int x = -(int)playerSafeRadius; x <= (int)playerSafeRadius; x++)
        {
            for (int y = -(int)playerSafeRadius; y <= (int)playerSafeRadius; y++)
            {
                ClearWall(x, y, halfW, halfH);
            }
        }
    }

    void SetWall(int worldX, int worldY, int halfW, int halfH)
    {
        int gx = worldX + halfW;
        int gy = worldY + halfH;
        if (gx >= 0 && gx < wallGrid.GetLength(0) && gy >= 0 && gy < wallGrid.GetLength(1))
        {
            wallGrid[gx, gy] = true;
        }
    }

    void ClearWall(int worldX, int worldY, int halfW, int halfH)
    {
        int gx = worldX + halfW;
        int gy = worldY + halfH;
        if (gx >= 0 && gx < wallGrid.GetLength(0) && gy >= 0 && gy < wallGrid.GetLength(1))
        {
            wallGrid[gx, gy] = false;
        }
    }
}