using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemySpawnPositionSystem : ISystem
{
    private float elapsedTime;
    private Unity.Mathematics.Random randomGenerator;
    private EntityArchetype spawnPointArchetype;

    public void OnCreate(ref SystemState state)
    {
        elapsedTime = 0;

        randomGenerator = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));

        spawnPointArchetype = state.EntityManager.CreateArchetype(
                  typeof(LocalTransform),
                  typeof(SpawnPointComponent),
                  typeof(PositionComponent),
                  typeof(TimeCounterComponent)
                );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        elapsedTime += deltaTime;

        if (elapsedTime < 5) return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) return;
        if (!SystemAPI.TryGetSingletonEntity<MapEntityComponent>(out Entity mapEntity)) return;

        MapEntityGameStateComponent gameState = state.EntityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

        if (gameState.IsNightmareActive) return;

        PositionComponent playerPosition = state.EntityManager.GetComponentData<PositionComponent>(playerEntity);
        LevelComponent playerLevel = state.EntityManager.GetComponentData<LevelComponent>(playerEntity);
        TimeCounterComponent gameTime = state.EntityManager.GetComponentData<TimeCounterComponent>(mapEntity);

        int monsterLevel = GetMonsterLevel(gameTime.ElapsedTime, playerLevel.Level);
        int roundLevel = GetRoundLevel(gameTime.ElapsedTime, playerLevel.Level);

        int batCount = 7 * (int)math.pow(2, roundLevel - 1);
        int crabCount = 7 * (int)math.pow(2, roundLevel - 2);
        int ratCount = 7 * (int)math.pow(2, roundLevel - 3);
        int slimeCount = 7 * (int)math.pow(2, roundLevel - 4);
        int golemCount = 7 * (int)math.pow(2, roundLevel - 5);

        int totalMonsterCount = batCount + crabCount + ratCount + slimeCount + golemCount;

        NativeArray<float2> spawnPoints = new NativeArray<float2>(totalMonsterCount, Allocator.TempJob);
        GenerateSpawnPointsJob job = new GenerateSpawnPointsJob
        {
            playerPosition = playerPosition.Position,
            spawnPointsCount = totalMonsterCount,
            roundLevel = roundLevel,
            random = randomGenerator,
            spawnPoints = spawnPoints
        };

        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Entity spawnPointEntity = state.EntityManager.CreateEntity(spawnPointArchetype);
            state.EntityManager.SetComponentData(spawnPointEntity, new PositionComponent
            {
                Position = new float2(spawnPoints[i].x, spawnPoints[i].y)
            });

            MonsterType monsterType;
            if (i < batCount)
            {
                monsterType = MonsterType.Bat;
            }
            else if (i < batCount + crabCount)
            {
                monsterType = MonsterType.Crab;
            }
            else if (i < batCount + crabCount + ratCount)
            {
                monsterType = MonsterType.Rat;
            }
            else if (i < batCount + crabCount + ratCount + slimeCount)
            {
                monsterType = MonsterType.Slime;
            }
            else
            {
                monsterType = MonsterType.Golem;
            }

            state.EntityManager.SetComponentData(spawnPointEntity, new SpawnPointComponent
            {
                SpawnLevel = monsterLevel,
                Type = SpawnType.None,
                Parent = Entity.Null,
                Difficulty = MonsterDifficulty.None,
                MonsterType = monsterType
            });

            state.EntityManager.SetComponentData(spawnPointEntity, new LocalTransform
            {
                Position = new float3(spawnPoints[i].x, spawnPoints[i].y, 0)
            });
            state.EntityManager.SetComponentData(spawnPointEntity, new TimeCounterComponent
            {
                ElapsedTime = 0,
                EndTime = 4,
                isInfinite = false,
            });
        }

        spawnPoints.Dispose();

        elapsedTime = 0;
        randomGenerator.NextUInt();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    private int GetRoundLevel(float elapsedTime, int playerLevel)
    {
        int enemyLevel = GetMonsterLevel(elapsedTime, playerLevel);

        if (enemyLevel <= 5) return 1;
        if (enemyLevel <= 10) return 2;
        if (enemyLevel <= 15) return 3;
        if (enemyLevel <= 20) return 4;
        return 5;
    }

    [BurstCompile]
    private int GetMonsterLevel(float elapsedTime, int playerLevel)
    {
        int baseLevel = 1;
        float timeFactor = 0.2f;
        float playerLevelFactor = 0.2f;

        return Math.Max((int)(baseLevel + (elapsedTime * timeFactor) + (playerLevel * playerLevelFactor)), 1);
    }
}

[BurstCompile]
public partial struct GenerateSpawnPointsJob : IJob
{
    public float2 playerPosition;
    public int spawnPointsCount;
    public int roundLevel;
    public Unity.Mathematics.Random random;
    public NativeArray<float2> spawnPoints;

    public void Execute()
    {
        int minDistance = 3;
        int maxDistance = 6 + ((roundLevel - 1) * 2);

        float2 spawnPoint;
        for (int i = 0; i < spawnPointsCount; i++)
        {
            do
            {
                int xOffset = random.NextInt(-maxDistance, maxDistance + 1);
                int yOffset = random.NextInt(-maxDistance, maxDistance + 1);

                spawnPoint = playerPosition + new float2(xOffset, yOffset);
            } while (math.abs(spawnPoint.x - playerPosition.x) < minDistance
                     || math.abs(spawnPoint.y - playerPosition.y) < minDistance
                     || math.abs(spawnPoint.x - playerPosition.x) > maxDistance
                     || math.abs(spawnPoint.y - playerPosition.y) > maxDistance);


            spawnPoints[i] = spawnPoint;
        }

    }
}
