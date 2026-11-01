
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

    // while the spawned objects are called "chunks", they're actually ground tiles


    // Start is called before the first frame update
    void Start()
    {
        playerPosition = playerTransform.position;
        playerCoordinates.x = Mathf.FloorToInt(playerPosition.x / chunkSize);
        playerCoordinates.y = Mathf.FloorToInt(playerPosition.z / chunkSize);
        chunkCoordinates = playerCoordinates;

        SpawnChunk(chunkCoordinates); // spawns the initial chunk
        GenerateSurroundingChunks(); // spawns the other 8 chunks around it
    }

    // Update is called once per frame
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
        Vector3 spawnPosition = new Vector3(coordinates.x * chunkSize + chunkSize / 2f, 0f, coordinates.y * chunkSize + chunkSize / 2f );

        GameObject newChunk = Instantiate(groundTile, spawnPosition, Quaternion.identity);

        generatedChunks.Add(coordinates, newChunk);
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
                Destroy(chunk.Value);
                chunksToRemove.Add(coordinates);
            }
        }

        foreach (Vector2Int coordinates in chunksToRemove)
        {
            generatedChunks.Remove(coordinates);
        }
    }
}
