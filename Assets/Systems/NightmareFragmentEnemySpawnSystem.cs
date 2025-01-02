using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

partial struct NightmareFragmentEnemySpawnSystem : ISystem
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

        if (elapsedTime < 7) return;

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) return;
        if (!SystemAPI.TryGetSingletonEntity<MapEntityComponent>(out Entity mapEntity)) return;

        MapEntityGameStateComponent gameState = state.EntityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

        if (!gameState.IsNightmareActive) return;

        foreach (var (nightmareFragment, entity) in SystemAPI.Query<RefRW<NightmareFragmentComponent>>().WithEntityAccess())
        {
            if (nightmareFragment.ValueRO.IsActive)
            {
                PositionComponent playerPosition = state.EntityManager.GetComponentData<PositionComponent>(playerEntity);

                if (nightmareFragment.ValueRO.Type == NightmareFragmentType.Endless_Siege)
                {
                    Entity foundEntity = Entity.Null;
                    foreach (var (endlessSiege, endlessSiegeEntity) in SystemAPI.Query<RefRW<NightmareFragmentEndlessSiegeComponent>>().WithEntityAccess())
                    {
                        if (endlessSiege.ValueRO.Parent == entity)
                        {
                            foundEntity = endlessSiegeEntity;
                            break;
                        }
                    }

                    if (foundEntity == Entity.Null) return;

                    NightmareFragmentEndlessSiegeComponent endlessSiegeFound = state.EntityManager.GetComponentData<NightmareFragmentEndlessSiegeComponent>(foundEntity);

                    if (endlessSiegeFound.Round > 5) return;

                    LocalTransform nightmareFragmentTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);

                    NativeArray<float2> spawnPoints = new NativeArray<float2>(70, Allocator.TempJob);
                    GenerateNightmareSpawnPointsJob job = new GenerateNightmareSpawnPointsJob
                    {
                        playerPosition = playerPosition.Position,
                        center = nightmareFragmentTransform.Position.xy,
                        spawnPointsCount = 70,
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
                        state.EntityManager.SetComponentData(spawnPointEntity, new SpawnPointComponent
                        {
                            SpawnLevel = (endlessSiegeFound.Round - 1) * 5 + 5,
                            Type = SpawnType.NightmareFragment,
                            Parent = entity,
                            Difficulty = MonsterDifficulty.None
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

                    endlessSiegeFound.Round++;
                    state.EntityManager.SetComponentData(foundEntity, endlessSiegeFound);
                }

                if (nightmareFragment.ValueRO.Type == NightmareFragmentType.Fury)
                {
                    Entity foundEntity = Entity.Null;
                    foreach (var (fury, furyEntity) in SystemAPI.Query<RefRW<NightmareFragmentFuryComponent>>().WithEntityAccess())
                    {
                        if (fury.ValueRO.Parent == entity)
                        {
                            foundEntity = furyEntity;
                            break;
                        }
                    }

                    if (foundEntity == Entity.Null) return;

                    NightmareFragmentFuryComponent furyFound = state.EntityManager.GetComponentData<NightmareFragmentFuryComponent>(foundEntity);

                    if (furyFound.BossSpawned) return;

                    TimeCounterComponent gameTime = state.EntityManager.GetComponentData<TimeCounterComponent>(mapEntity);

                    LocalTransform nightmareFragmentTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);

                    int bossCount = (int)(gameTime.ElapsedTime / 60) + 20;

                    NativeArray<float2> spawnPoints = new NativeArray<float2>(bossCount, Allocator.TempJob);
                    GenerateNightmareSpawnPointsJob job = new GenerateNightmareSpawnPointsJob
                    {
                        playerPosition = playerPosition.Position,
                        center = nightmareFragmentTransform.Position.xy,
                        spawnPointsCount = bossCount,
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
                        state.EntityManager.SetComponentData(spawnPointEntity, new SpawnPointComponent
                        {
                            SpawnLevel = (i + 1) * 5,
                            Type = SpawnType.NightmareFragment,
                            Parent = entity,
                            Difficulty = MonsterDifficulty.MiniBoss
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

                    furyFound.BossSpawned = true;
                    state.EntityManager.SetComponentData(foundEntity, furyFound);
                }
            }
        }

        elapsedTime = 0;
        randomGenerator.NextUInt();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

[BurstCompile]
public partial struct GenerateNightmareSpawnPointsJob : IJob
{
    public float2 playerPosition;
    public float2 center;
    public int spawnPointsCount;
    public Unity.Mathematics.Random random;
    public NativeArray<float2> spawnPoints;

    public void Execute()
    {
        for (int i = 0; i < spawnPointsCount; i++)
        {
            float2 randomPoint = new float2(random.NextFloat(-5, 5), random.NextFloat(-5, 5));
            spawnPoints[i] = center + randomPoint;
        }
    }
}
