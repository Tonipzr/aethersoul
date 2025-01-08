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
    [SerializeField]
    private GameObject shadowExplosion;
    [SerializeField]
    private GameObject fireTornado;
    [SerializeField]
    private GameObject shadowAA;
    [SerializeField]
    private GameObject windStorm;

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
                AirEscarlinaAA = authoring.airEscarlinaAA,
                ShadowExplosion = authoring.shadowExplosion,
                ShadowExplosion2 = authoring.shadowExplosion,
                FireTornado = authoring.fireTornado,
                ShadowAA = authoring.shadowAA,
                Windstorm = authoring.windStorm
            });
        }
    }
}
