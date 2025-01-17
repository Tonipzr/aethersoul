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
    public GameObject ShadowExplosion;
    public GameObject ShadowExplosion2;
    public GameObject FireTornado;
    public GameObject ShadowAA;
    public GameObject Windstorm;

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
            case 7:
            case 8:
                return ShadowExplosion;
            case 9:
                return FireTornado;
            case 10:
                return ShadowAA;
            case 11:
                return Windstorm;
            default:
                return null;
        }
    }
}
