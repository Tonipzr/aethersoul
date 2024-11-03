using Unity.Entities;

public struct WeatherComponent : IComponentData
{
    public WeatherType Weather;
}

public enum WeatherType
{
    Clear,
    Rain
}
