using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CheckpointStatusController : MonoBehaviour
{
    Entity entity;

    void SetRunes(bool status)
    {
        Transform runes = transform.GetChild(0);

        runes.gameObject.SetActive(status);
    }

    // Update is called once per frame
    void Update()
    {
        if (entity == null)
        {
            return;
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager.HasComponent<MapCheckpointEntityComponent>(entity))
        {
            MapCheckpointEntityComponent checkpointEntity = entityManager.GetComponentData<MapCheckpointEntityComponent>(entity);

            if (checkpointEntity.IsColliding)
            {
                SetRunes(true);
            }
            else
            {
                SetRunes(false);
            }
        }
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }
}
