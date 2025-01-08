using TMPro;
using Unity.Entities;
using UnityEngine;

public class IngameTimeManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        timeText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity mapEntity = entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();

        if (entityManager.HasComponent<MapEntityGameStateComponent>(mapEntity))
        {
            MapEntityGameStateComponent gameState = entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

            if (gameState.GameStarted)
            {
                timeText.gameObject.SetActive(true);
                TimeCounterComponent gameTime = entityManager.GetComponentData<TimeCounterComponent>(mapEntity);

                timeText.text = $"{gameTime.ElapsedTime:0.00}";
            }
            else
            {
                timeText.gameObject.SetActive(false);
            }
        }
    }
}
