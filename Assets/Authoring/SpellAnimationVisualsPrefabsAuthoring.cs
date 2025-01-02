using Unity.Entities;
using UnityEngine;

class SpellAnimationVisualsPrefabsAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject fireBallPrefab;
    [SerializeField]
    private GameObject flameNovaPrefab;
    [SerializeField]
    private GameObject meteorstormPrefab;
    [SerializeField]
    private GameObject infernoburstPrefab;
    [SerializeField]
    private GameObject gustPrefab;
    [SerializeField]
    private GameObject airEscarlinaAA;

    private class SpellAnimationVisualsPrefabsAuthoringBaker : Baker<SpellAnimationVisualsPrefabsAuthoring>
    {
        public override void Bake(SpellAnimationVisualsPrefabsAuthoring authoring)
        {
            Entity prefabEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(prefabEntity, new SpellAnimationVisualsPrefabs
            {
                Fireslash = authoring.fireBallPrefab,
                Flamenova = authoring.flameNovaPrefab,
                Meteorstorm = authoring.meteorstormPrefab,
                Infernoburst = authoring.infernoburstPrefab,
                Gust = authoring.gustPrefab,
                AirEscarlinaAA = authoring.airEscarlinaAA
            });
        }
    }
}
