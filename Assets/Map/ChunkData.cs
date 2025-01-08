using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ChunkData
{
    public Dictionary<Vector2Int, TileBase> tiles = new Dictionary<Vector2Int, TileBase>();
    public Dictionary<Vector2Int, GameObject> treePositions = new Dictionary<Vector2Int, GameObject>();
    public List<Vector2Int> checkpointPositions = new List<Vector2Int>();
    public Dictionary<Vector2Int, Entity> checkpointEntities = new Dictionary<Vector2Int, Entity>();
    public Dictionary<Vector2Int, bool> buffPositions = new Dictionary<Vector2Int, bool>();
    public Dictionary<Vector2Int, Entity> buffEntities = new Dictionary<Vector2Int, Entity>();
    public Dictionary<Vector2Int, bool> nightmareFragmentPositions = new Dictionary<Vector2Int, bool>();
    public Dictionary<Vector2Int, Entity> nightmareFragmentEntities = new Dictionary<Vector2Int, Entity>();

    public List<GameObject> instantiatedTrees = new List<GameObject>();
    public List<GameObject> instantiatedCheckpoints = new List<GameObject>();
    public List<GameObject> instantiatedBuffs = new List<GameObject>();
    public List<GameObject> instantiatedNightmareFragments = new List<GameObject>();
}