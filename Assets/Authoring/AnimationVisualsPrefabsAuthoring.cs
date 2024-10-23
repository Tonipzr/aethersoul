using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimationVisualsPrefabsAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject escarlinaPrefab;


    private class AnimationVisualsPrefabsBaker : Baker<AnimationVisualsPrefabsAuthoring>
    {
        public override void Bake(AnimationVisualsPrefabsAuthoring authoring)
        {
            Entity prefabEntity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(prefabEntity, new AnimationVisualsPrefabs { Escarlina = authoring.escarlinaPrefab });
        }
    }
}