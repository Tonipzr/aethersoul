using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHandler : MonoBehaviour
{
    [SerializeField] Grid map;
    [SerializeField] Tilemap world;
    [SerializeField] TileBase grassTile;
    [SerializeField] TileBase flowerTile;
    [SerializeField] TileBase stoneTile;
    [SerializeField] GameObject Tree1;
    [SerializeField] GameObject Tree2;
    [SerializeField] GameObject Tree3;

    [SerializeField] GameObject chest;
    [SerializeField] GameObject chestOpen;

    [SerializeField] GameObject checkPoint;

    [SerializeField] Camera mainCamera;
    [SerializeField] float viewDistance = 20f;

    private int chunkSize = 10;
    private Dictionary<Vector2Int, ChunkData> generatedChunks = new Dictionary<Vector2Int, ChunkData>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateChunkAtPosition(0, 0);
    }

    void GenerateChunkAtPosition(int chunkX, int chunkY)
    {
        Debug.Log("Generating chunk at " + chunkX + ", " + chunkY);
        Vector2Int chunkPos = new Vector2Int(chunkX, chunkY);

        if (generatedChunks.ContainsKey(chunkPos))
        {
            LoadChunk(chunkPos);
            return;
        }

        ChunkData newChunkData = new ChunkData();

        float offsetX = Random.Range(-1000f, 1000f);
        float offsetY = Random.Range(-1000f, 1000f);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int tileX = chunkX * chunkSize + x;
                int tileY = chunkY * chunkSize + y;

                float noiseValue = Mathf.PerlinNoise((tileX + offsetX) * 0.1f, (tileY + offsetY) * 0.1f);

                TileBase tileToPlace;
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);
                Vector2Int localPos = new Vector2Int(x, y);
                if (noiseValue < 0.3f)
                {
                    tileToPlace = stoneTile;
                }
                else if (noiseValue < 0.5f)
                {
                    tileToPlace = flowerTile;
                }
                else
                {
                    tileToPlace = grassTile;

                    if (Random.Range(0f, 1f) < 0.01f)
                    {
                        GameObject treePrefab = Random.Range(0f, 1f) < 0.5f ? Tree1 : Random.Range(0f, 1f) < 0.5f ? Tree2 : Tree3;
                        GameObject treeInstance = Instantiate(treePrefab, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                        newChunkData.treePositions.Add(localPos, treePrefab);
                        newChunkData.instantiatedTrees.Add(treeInstance);
                    }
                }

                newChunkData.tiles[localPos] = tileToPlace;

                if (Random.Range(0f, 75f) < 0.01f)
                {
                    GameObject checkPointInstance = Instantiate(checkPoint, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                    newChunkData.checkpointPositions.Add(localPos);
                    newChunkData.instantiatedCheckpoints.Add(checkPointInstance);
                }

                world.SetTile(tilePos, tileToPlace);
            }
        }

        generatedChunks[chunkPos] = newChunkData;
    }

    void LoadChunk(Vector2Int chunkPos)
    {
        ChunkData chunkData = generatedChunks[chunkPos];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector3Int tilePos = new Vector3Int(chunkPos.x * chunkSize + x, chunkPos.y * chunkSize + y, 0);

                if (chunkData.tiles.TryGetValue(localPos, out TileBase tileToPlace))
                {
                    world.SetTile(tilePos, tileToPlace);
                }
            }
        }

        foreach (var treePosition in chunkData.treePositions)
        {
            Vector2Int pos = treePosition.Key;
            GameObject treePrefab = treePosition.Value;
            GameObject treeInstance = Instantiate(treePrefab, map.CellToWorld(new Vector3Int(chunkPos.x * chunkSize + pos.x, chunkPos.y * chunkSize + pos.y, 0)), Quaternion.identity, world.transform);
            chunkData.instantiatedTrees.Add(treeInstance);
        }

        foreach (Vector2Int pos in chunkData.checkpointPositions)
        {
            GameObject checkPointInstance = Instantiate(checkPoint, map.CellToWorld(new Vector3Int(chunkPos.x * chunkSize + pos.x, chunkPos.y * chunkSize + pos.y, 0)), Quaternion.identity, world.transform);
            chunkData.instantiatedCheckpoints.Add(checkPointInstance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector2Int cameraChunkPos = GetChunkPosition(cameraPosition);

        int viewRange = Mathf.CeilToInt(viewDistance / chunkSize);

        HashSet<Vector2Int> visibleChunks = new HashSet<Vector2Int>();

        for (int x = -viewRange; x <= viewRange; x++)
        {
            for (int y = -viewRange; y <= viewRange; y++)
            {
                Vector2Int neighborChunkPos = cameraChunkPos + new Vector2Int(x, y);
                visibleChunks.Add(neighborChunkPos);

                if (!generatedChunks.ContainsKey(neighborChunkPos))
                {
                    GenerateChunkAtPosition(neighborChunkPos.x, neighborChunkPos.y);
                }
                else if (!IsChunkVisibleOnTilemap(neighborChunkPos))
                {
                    LoadChunk(neighborChunkPos);
                }
            }
        }

        foreach (var chunkPos in generatedChunks.Keys)
        {
            if (!visibleChunks.Contains(chunkPos) && IsChunkVisibleOnTilemap(chunkPos))
            {
                UnloadChunk(chunkPos);
            }
        }
    }

    bool IsChunkVisibleOnTilemap(Vector2Int chunkPos)
    {
        Vector3Int tilePos = new Vector3Int(chunkPos.x * chunkSize, chunkPos.y * chunkSize, 0);
        return world.HasTile(tilePos);
    }

    void UnloadChunk(Vector2Int chunkPos)
    {
        if (generatedChunks.TryGetValue(chunkPos, out ChunkData chunkData))
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3Int tilePos = new Vector3Int(chunkPos.x * chunkSize + x, chunkPos.y * chunkSize + y, 0);
                    world.SetTile(tilePos, null);
                }
            }

            foreach (GameObject tree in chunkData.instantiatedTrees)
            {
                Destroy(tree);
            }
            chunkData.instantiatedTrees.Clear();

            foreach (GameObject checkpoint in chunkData.instantiatedCheckpoints)
            {
                Destroy(checkpoint);
            }
            chunkData.instantiatedCheckpoints.Clear();
        }
    }

    private Vector2Int GetChunkPosition(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkSize);
        int chunkY = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }
}
