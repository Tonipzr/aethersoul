using Unity.Entities;
using UnityEngine;

public class NightmareFragmentController : MonoBehaviour
{
    Entity entity;
    bool isActive = false;
    bool isCompleted = false;

    [SerializeField]
    private GameObject Chest;
    [SerializeField]
    private GameObject OpenChest;
    [SerializeField]
    private GameObject Closures;

    void Start()
    {
        Chest.SetActive(true);
        OpenChest.SetActive(false);
        Closures.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (entity == null)
        {
            return;
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (entityManager.HasComponent<NightmareFragmentComponent>(entity))
        {
            NightmareFragmentComponent nightmareComponent = entityManager.GetComponentData<NightmareFragmentComponent>(entity);

            if (nightmareComponent.IsActive)
            {
                isActive = true;
                Closures.SetActive(true);
            }
            else
            {
                isActive = false;
                Closures.SetActive(false);
            }

            if (nightmareComponent.IsCompleted)
            {
                isCompleted = true;
                Chest.SetActive(false);
                OpenChest.SetActive(true);
            }
            else
            {
                isCompleted = false;
                Chest.SetActive(true);
                OpenChest.SetActive(false);
            }
        }
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }
}
