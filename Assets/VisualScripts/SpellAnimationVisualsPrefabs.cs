using Unity.Entities;
using UnityEngine;

public class SpellAnimationVisualsPrefabs : IComponentData
{
    public GameObject Fireslash;
    public GameObject Flamenova;
    public GameObject Meteorstorm;
    public GameObject Infernoburst;
    public GameObject Gust;
    public GameObject AirEscarlinaAA;

    public GameObject GetSpellPrefab(int spellID)
    {
        switch (spellID)
        {
            case 1:
                return Fireslash;
            case 2:
                return Flamenova;
            case 3:
                return Meteorstorm;
            case 4:
                return Infernoburst;
            case 5:
                return Gust;
            case 6:
                return AirEscarlinaAA;
            default:
                return null;
        }
    }
}
