using Unity.Entities;

public struct WeatherUpdated : IComponentData
{
    public WeatherType NewWeather;
}
