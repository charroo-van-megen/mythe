using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkerGenerator : MonoBehaviour
{
    public enum GridCell
    {
        Floor,
        Wall,
        Empty
    }

    public GridCell[,] grid;

    public List<Walker> walkers;

    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public int mapWidth = 20;
    public int mapHeight = 20;

    public int maxWalkers = 10;
    public int tileCount = 0;
    public float fillPercent = 0.5f;

    public Node nodePrefab;
    public List<Node> nodeList;

    public NPC_Controller npcPrefab;

    private bool canDrawGizmos;

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new GridCell[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                grid[x, z] = GridCell.Empty;
            }
        }

        walkers = new List<Walker>();

        Vector2Int center = new Vector2Int(mapWidth / 2, mapHeight / 2);
        Vector3 worldPos = new Vector3(center.x, 0f, center.y);
        Walker newWalker = new Walker(new Vector3(center.x, 0f, center.y), GetDirection(), 0.5f);
        grid[center.x, center.y] = GridCell.Floor;
        Instantiate(floorPrefab, worldPos, Quaternion.identity, transform);
        walkers.Add(newWalker);

        tileCount++;
        CreateFloors();
    }

    public Vector3 GetDirection()
    {
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0: return Vector3.forward; // Z+
            case 1: return Vector3.back;    // Z-
            case 2: return Vector3.left;    // X-
            case 3: return Vector3.right;   // X+
        }
        return Vector3.zero;
    }

    void CreateFloors()
    {
        while ((float)tileCount / (float)(mapWidth * mapHeight) < fillPercent)
        {
            foreach (Walker walker in walkers)
            {
                int x = Mathf.RoundToInt(walker.pos.x);
                int z = Mathf.RoundToInt(walker.pos.z);

                if (grid[x, z] != GridCell.Floor)
                {
                    Instantiate(floorPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
                    grid[x, z] = GridCell.Floor;
                    tileCount++;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
        }

        CreateWalls();
    }

    void ChanceToRemove()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].chanceToChange && walkers.Count > 1)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (Random.value < walkers[i].chanceToChange)
            {
                walkers[i].dir = GetDirection();
            }
        }
    }

    void ChanceToCreate()
    {
        int currentCount = walkers.Count;
        for (int i = 0; i < currentCount; i++)
        {
            if (Random.value < walkers[i].chanceToChange && walkers.Count < maxWalkers)
            {
                walkers.Add(new Walker(walkers[i].pos, GetDirection(), 0.5f));
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            walkers[i].pos += walkers[i].dir;

            walkers[i].pos.x = Mathf.Clamp(walkers[i].pos.x, 1, mapWidth - 2);
            walkers[i].pos.z = Mathf.Clamp(walkers[i].pos.z, 1, mapHeight - 2);
        }
    }

    void CreateWalls()
    {
        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int z = 1; z < mapHeight - 1; z++)
            {
                if (grid[x, z] == GridCell.Floor)
                {
                    TryPlaceWall(x + 1, z);
                    TryPlaceWall(x - 1, z);
                    TryPlaceWall(x, z + 1);
                    TryPlaceWall(x, z - 1);
                }
            }
        }

        CreateNodes();
    }

    void TryPlaceWall(int x, int z)
    {
        if (grid[x, z] == GridCell.Empty)
        {
            Instantiate(wallPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
            grid[x, z] = GridCell.Wall;
        }
    }

    void CreateNodes()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                if (grid[x, z] == GridCell.Floor)
                {
                    Node node = Instantiate(nodePrefab, new Vector3(x + 0.5f, 0f, z + 0.5f), Quaternion.identity);
                    nodeList.Add(node);
                }
            }
        }

        CreateConnections();
    }

    void CreateConnections()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = i + 1; j < nodeList.Count; j++)
            {
                if (Vector3.Distance(nodeList[i].transform.position, nodeList[j].transform.position) <= 1.1f)
                {
                    nodeList[i].connections.Add(nodeList[j]);
                    nodeList[j].connections.Add(nodeList[i]);
                }
            }
        }

        canDrawGizmos = true;
        SpawnAI();
    }

    void SpawnAI()
    {
        Node randNode = nodeList[Random.Range(0, nodeList.Count)];
        NPC_Controller npc = Instantiate(npcPrefab, randNode.transform.position, Quaternion.identity);
        npc.currentNode = randNode;
    }
}

[System.Serializable]
public class Walker
{
    public Vector3 pos;
    public Vector3 dir;
    public float chanceToChange;

    public Walker(Vector3 p, Vector3 d, float c)
    {
        pos = p;
        dir = d;
        chanceToChange = c;
    }
}
