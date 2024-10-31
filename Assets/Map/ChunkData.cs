using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ChunkData
{
    public Dictionary<Vector2Int, TileBase> tiles = new Dictionary<Vector2Int, TileBase>();
    public Dictionary<Vector2Int, GameObject> treePositions = new Dictionary<Vector2Int, GameObject>();
    public List<Vector2Int> checkpointPositions = new List<Vector2Int>();

    public List<GameObject> instantiatedTrees = new List<GameObject>();
    public List<GameObject> instantiatedCheckpoints = new List<GameObject>();
}