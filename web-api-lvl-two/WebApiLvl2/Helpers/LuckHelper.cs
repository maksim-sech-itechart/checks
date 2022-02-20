namespace WebApiLvl2.Helpers;

public class LuckHelper
{
    private readonly Random _random = new();

    public bool Lucky => (_random.NextDouble() * 100) > 49;
}