using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHandler : MonoBehaviour
{
    [SerializeField] Grid map;
    [SerializeField] Tilemap world;
    [SerializeField] TileBase grassTile;
    [SerializeField] TileBase flowerTile;
    [SerializeField] TileBase stoneTile;
    [SerializeField] GameObject spawnPointTile;
    [SerializeField] GameObject Tree1;
    [SerializeField] GameObject Tree2;
    [SerializeField] GameObject Tree3;
    [SerializeField] GameObject chest;
    [SerializeField] GameObject chestOpen;
    [SerializeField] GameObject checkPoint;
    [SerializeField] GameObject buff;


    [SerializeField] Camera mainCamera;
    [SerializeField] float viewDistance = 20f;

    private int chunkSize = 10;
    private Dictionary<Vector2Int, ChunkData> generatedChunks = new Dictionary<Vector2Int, ChunkData>();
    private Dictionary<Vector2Int, GameObject> instantiatedSpawnPoints = new Dictionary<Vector2Int, GameObject>();

    private EntityManager _entityManager;

    // Start is called before the first frame update
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GenerateChunkAtPosition(0, 0);
    }

    void GenerateChunkAtPosition(int chunkX, int chunkY)
    {
        // Debug.Log("Generating chunk at " + chunkX + ", " + chunkY);
        Vector2Int chunkPos = new Vector2Int(chunkX, chunkY);

        if (generatedChunks.ContainsKey(chunkPos))
        {
            LoadChunk(chunkPos);
            return;
        }

        ChunkData newChunkData = new ChunkData();

        float offsetX = UnityEngine.Random.Range(-1000f, 1000f);
        float offsetY = UnityEngine.Random.Range(-1000f, 1000f);

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

                    if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    {
                        GameObject treePrefab = UnityEngine.Random.Range(0f, 1f) < 0.5f ? Tree1 : UnityEngine.Random.Range(0f, 1f) < 0.5f ? Tree2 : Tree3;
                        GameObject treeInstance = Instantiate(treePrefab, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                        newChunkData.treePositions.Add(localPos, treePrefab);
                        newChunkData.instantiatedTrees.Add(treeInstance);
                    }
                }

                newChunkData.tiles[localPos] = tileToPlace;

                if (UnityEngine.Random.Range(0f, 75f) < 0.01f)
                {
                    GameObject checkPointInstance = Instantiate(checkPoint, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                    newChunkData.checkpointPositions.Add(localPos);
                    newChunkData.instantiatedCheckpoints.Add(checkPointInstance);

                    Entity entity = _entityManager.CreateEntity(
                        typeof(MapCheckpointEntityComponent),
                        typeof(PhysicsWorldIndex),
                        typeof(LocalTransform),
                        typeof(PhysicsCollider),
                        typeof(PhysicsDamping),
                        typeof(PhysicsGravityFactor),
                        typeof(PhysicsMass),
                        typeof(PhysicsVelocity)
                    );
                    _entityManager.SetComponentData(entity, new MapCheckpointEntityComponent { Coordinates = new Vector2Int(tileX, tileY), IsColliding = false, IsVisited = false });

                    _entityManager.SetComponentData(entity, new LocalTransform
                    {
                        Position = new float3(tileX, tileY, 0),
                        Rotation = quaternion.identity,
                        Scale = 1
                    });

                    var collider = new PhysicsCollider
                    {
                        Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                        {
                            Center = new float3(0, 0, 0),
                            Size = new float3(1, 1, 1),
                            Orientation = quaternion.identity,
                            BevelRadius = 0,
                        }, new CollisionFilter
                        {
                            BelongsTo = 16,
                            CollidesWith = 1,
                            GroupIndex = 0
                        })
                    };
                    collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                    _entityManager.SetComponentData(entity, collider);

                    _entityManager.SetComponentData(entity, new PhysicsDamping
                    {
                        Linear = 0.01f,
                        Angular = 0.05f
                    });

                    _entityManager.SetComponentData(entity, new PhysicsGravityFactor
                    {
                        Value = 0
                    });

                    _entityManager.SetComponentData(entity, new PhysicsMass
                    {
                        InverseInertia = 6,
                        InverseMass = 1,
                        AngularExpansionFactor = 0,
                        InertiaOrientation = quaternion.identity,
                    });

                    _entityManager.SetComponentData(entity, new PhysicsVelocity
                    {
                        Linear = new float3(0, 0, 0),
                        Angular = new float3(0, 0, 0)
                    });

                    CheckpointStatusController checkpontStatusController = checkPointInstance.GetComponent<CheckpointStatusController>();
                    checkpontStatusController.SetEntity(entity);

                    newChunkData.checkpointEntities[localPos] = entity;
                }

                if (UnityEngine.Random.Range(0f, 75f) < 0.01f)
                {
                    GameObject buffInstance = Instantiate(buff, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                    newChunkData.buffPositions.Add(localPos, false);
                    newChunkData.instantiatedBuffs.Add(buffInstance);

                    Entity entity = _entityManager.CreateEntity(typeof(MapBuffEntityComponent));
                    _entityManager.SetComponentData(entity, new MapBuffEntityComponent { Coordinates = new Vector2Int(tileX, tileY), IsUsed = false });

                    BuffStatusController buffStatusController = buffInstance.GetComponent<BuffStatusController>();
                    buffStatusController.SetEntity(entity);

                    newChunkData.buffEntities[localPos] = entity;
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

            CheckpointStatusController checkpointStatusController = checkPointInstance.GetComponent<CheckpointStatusController>();

            if (chunkData.checkpointEntities.TryGetValue(pos, out Entity entity))
            {
                checkpointStatusController.SetEntity(entity);
            }
        }

        foreach (var buffPosition in chunkData.buffPositions)
        {
            Vector2Int pos = buffPosition.Key;
            GameObject buffInstance = Instantiate(buff, map.CellToWorld(new Vector3Int(chunkPos.x * chunkSize + pos.x, chunkPos.y * chunkSize + pos.y, 0)), Quaternion.identity, world.transform);
            chunkData.instantiatedBuffs.Add(buffInstance);

            BuffStatusController buffStatusController = buffInstance.GetComponent<BuffStatusController>();

            if (chunkData.buffEntities.TryGetValue(pos, out Entity entity))
            {
                buffStatusController.SetEntity(entity);
            }
            else
            {
                buffStatusController.SetIsUsed(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector2Int cameraChunkPos = GetChunkPosition(cameraPosition);

        Entity mapEntity = _entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();
        MapEntityPlayerAtChunkComponent playerAtChunk = _entityManager.GetComponentData<MapEntityPlayerAtChunkComponent>(mapEntity);

        if (playerAtChunk.PlayerAtChunk != cameraChunkPos)
        {
            playerAtChunk.PlayerAtChunk = cameraChunkPos;
            _entityManager.SetComponentData(mapEntity, playerAtChunk);
        }

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

        EntityQuery spawnPointQuery = _entityManager.CreateEntityQuery(typeof(SpawnPointComponent));
        NativeArray<Entity> spawnPointEntities = spawnPointQuery.ToEntityArray(Allocator.Temp);

        List<Vector2Int> visitedSpawns = new List<Vector2Int>();
        foreach (Entity entity in spawnPointEntities)
        {
            PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(entity);
            Vector3Int tilePos = new Vector3Int((int)positionComponent.Position.x, (int)positionComponent.Position.y, 0);
            Vector2Int localPos = new Vector2Int((int)positionComponent.Position.x, (int)positionComponent.Position.y);

            if (!instantiatedSpawnPoints.ContainsKey(localPos))
            {
                GameObject spawnInstance = Instantiate(spawnPointTile, map.CellToWorld(tilePos), Quaternion.identity, world.transform);
                instantiatedSpawnPoints[localPos] = spawnInstance;
            }

            visitedSpawns.Add(localPos);
        }

        foreach (var spawn in instantiatedSpawnPoints.ToList())
        {
            if (!visitedSpawns.Contains(spawn.Key))
            {
                Destroy(spawn.Value);
                instantiatedSpawnPoints.Remove(spawn.Key);
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

            foreach (GameObject buff in chunkData.instantiatedBuffs)
            {
                Destroy(buff);
            }
            chunkData.instantiatedBuffs.Clear();
        }
    }

    private Vector2Int GetChunkPosition(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / chunkSize);
        int chunkY = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }

    public TileBase GetTileAtPosition(float2 position)
    {
        Vector3Int tilePos = world.WorldToCell(new Vector3(position.x, position.y, 0));
        return world.GetTile(tilePos);
    }
}
