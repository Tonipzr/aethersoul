using Unity.Entities;
using UnityEngine;

public class AnimationVisualsPrefabsAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject escarlinaPrefab;
    [SerializeField]
    private GameObject Bat;
    [SerializeField]
    private GameObject Crab;
    [SerializeField]
    private GameObject Golem;
    [SerializeField]
    private GameObject Rat;
    [SerializeField]
    private GameObject Slime;
    [SerializeField]
    private GameObject Boss;
    [SerializeField]
    private GameObject GreenShardExperience;
    [SerializeField]
    private GameObject BlueShardExperience;
    [SerializeField]
    private GameObject PinkShardExperience;
    [SerializeField]
    private GameObject PurpleShardExperience;
    [SerializeField]
    private GameObject RedShardExperience;


    private class AnimationVisualsPrefabsBaker : Baker<AnimationVisualsPrefabsAuthoring>
    {
        public override void Bake(AnimationVisualsPrefabsAuthoring authoring)
        {
            Entity prefabEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(prefabEntity, new AnimationVisualsPrefabs
            {
                Escarlina = authoring.escarlinaPrefab,
                Bat = authoring.Bat,
                Crab = authoring.Crab,
                Golem = authoring.Golem,
                Rat = authoring.Rat,
                Slime = authoring.Slime,
                Boss = authoring.Boss,
                GreenShardExperience = authoring.GreenShardExperience,
                BlueShardExperience = authoring.BlueShardExperience,
                PinkShardExperience = authoring.PinkShardExperience,
                PurpleShardExperience = authoring.PurpleShardExperience,
                RedShardExperience = authoring.RedShardExperience
            });
        }
    }
}