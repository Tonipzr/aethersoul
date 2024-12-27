using Unity.Entities;
using UnityEngine;

public class BuffStatusController : MonoBehaviour
{
    Entity entity;
    bool isUsed = true;

    // Update is called once per frame
    void Update()
    {
        if (entity == null || isUsed == false)
        {
            return;
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager.HasComponent<MapBuffEntityComponent>(entity))
        {
            MapBuffEntityComponent buffEntity = entityManager.GetComponentData<MapBuffEntityComponent>(entity);

            if (buffEntity.IsUsed)
            {
                Transform runes = transform.GetChild(0);

                runes.gameObject.SetActive(false);

                SetIsUsed(true);
            }
        }
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }

    public void SetIsUsed(bool isUsed)
    {
        this.isUsed = isUsed;
    }
}
