
using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    Vector2Int playerCoordinates;
    Vector2Int chunkCoordinates;
    private Vector3 playerPosition;
    private Vector3 spawnPosition;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject groundTile;
    [SerializeField] private float chunkSize = 10f;
    Dictionary<Vector2Int, GameObject> generatedChunks = new Dictionary<Vector2Int, GameObject>();
    GameObject newChunk;
    [SerializeField] private int renderDistance = 1;

    [SerializeField] private GameObject treePrefab;
    [SerializeField] private int treesPerChunk = 1;
    [SerializeField] private float treeSpacing = 2f;
    [SerializeField, Range(0f, 1f)] private float treeSpawnChance = 0.4f;
    [SerializeField] private int worldSeed = 12345;

    Dictionary<Vector2Int, HashSet<int>> destroyedTrees = new Dictionary<Vector2Int, HashSet<int>>();

    // while the spawned objects are called "chunks", they're actually ground tiles

    void Start()
    {
        playerPosition = playerTransform.position;
        playerCoordinates.x = Mathf.FloorToInt(playerPosition.x / chunkSize);
        playerCoordinates.y = Mathf.FloorToInt(playerPosition.z / chunkSize);
        chunkCoordinates = playerCoordinates;

        SpawnChunk(chunkCoordinates); // spawns the initial chunk
        GenerateSurroundingChunks(); // spawns the other 8 chunks around it
    }

    void Update()
    {
        playerPosition = playerTransform.position;

        playerCoordinates.x = Mathf.FloorToInt(playerPosition.x / chunkSize); // converting player's poisition into coordinates
        playerCoordinates.y = Mathf.FloorToInt(playerPosition.z / chunkSize);

        if (chunkCoordinates != playerCoordinates) // check to see if player has moved into another chunk
        {
            Debug.Log("Entered new chunk");
            chunkCoordinates = playerCoordinates;

            if (!generatedChunks.ContainsKey(chunkCoordinates)) // generates new chunk if there isn't already a matching chunk in the dictionary
            {
                SpawnChunk(chunkCoordinates);
            }

            GenerateSurroundingChunks();
            DespawnDistantChunks();
        }
    }

    void SpawnChunk(Vector2Int coordinates)
    {
        Vector3 spawnPosition = new Vector3(coordinates.x * chunkSize + chunkSize / 2f, 0f, coordinates.y * chunkSize + chunkSize / 2f);

        GameObject newChunk = Instantiate(groundTile, spawnPosition, Quaternion.identity);

        generatedChunks.Add(coordinates, newChunk);

        SpawnTrees(coordinates, newChunk);
    }

    void SpawnTrees(Vector2Int coordinates, GameObject chunk)
    {
        if (treePrefab == null || treesPerChunk <= 0 || chunkSize <= 1f)
        {
            return;
        }

        // same seed for the same chunk, so the trees won't move around when coming back
        int seed = unchecked(worldSeed + coordinates.x * 73856093 + coordinates.y * 19349663);
        System.Random random = new System.Random(seed);

        if (random.NextDouble() > treeSpawnChance) // some chunks won't have trees
        {
            return;
        }

        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < treesPerChunk; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = 0.5f + (float)random.NextDouble() * (chunkSize - 1f);
                float z = 0.5f + (float)random.NextDouble() * (chunkSize - 1f);

                Vector3 position = new Vector3(coordinates.x * chunkSize + x, 0f, coordinates.y * chunkSize + z);

                bool tooClose = false;

                foreach (Vector3 otherPosition in positions)
                {
                    if (Vector3.Distance(position, otherPosition) < treeSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    positions.Add(position);

                    // still generate the position even if this tree was destroyed before
                    if (!IsTreeDestroyed(coordinates, i))
                    {
                        float rotation = (float)random.NextDouble() * 360f;

                        GameObject tree = Instantiate(treePrefab, position, Quaternion.Euler(0, rotation, 0), chunk.transform);

                        Tree treeScript = tree.GetComponent<Tree>();

                        if (treeScript != null)
                        {
                            treeScript.SetTreeInfo(this, coordinates, i);
                        }
                    }

                    break;
                }
            }
        }
    }

    bool IsTreeDestroyed(Vector2Int coordinates, int treeID)
    {
        if (destroyedTrees.ContainsKey(coordinates))
        {
            return destroyedTrees[coordinates].Contains(treeID);
        }

        return false;
    }

    public void RegisterDestroyedTree(Vector2Int coordinates, int treeID)
    {
        if (!destroyedTrees.ContainsKey(coordinates))
        {
            destroyedTrees.Add(coordinates, new HashSet<int>());
        }

        destroyedTrees[coordinates].Add(treeID); // remember this tree was chopped down
    }

    void GenerateSurroundingChunks()
    {
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int targetCoordinates = chunkCoordinates + new Vector2Int(x, y);

                if (!generatedChunks.ContainsKey(targetCoordinates))
                {
                    SpawnChunk(targetCoordinates);
                }
            }
        }
    }

    void DespawnDistantChunks()
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, GameObject> chunk in generatedChunks)
        {
            Vector2Int coordinates = chunk.Key;

            if (Mathf.Abs(coordinates.x - chunkCoordinates.x) > renderDistance ||
                Mathf.Abs(coordinates.y - chunkCoordinates.y) > renderDistance)
            {
                Destroy(chunk.Value); // trees get removed too since they're children of the chunk
                chunksToRemove.Add(coordinates);
            }
        }

        foreach (Vector2Int coordinates in chunksToRemove)
        {
            generatedChunks.Remove(coordinates);
        }
    }
}
