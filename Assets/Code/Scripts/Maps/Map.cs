using System.Collections.Generic;
using System.ComponentModel;

public enum Map
{
    MarcinK,
    Test1,
    Test2,
    Test3,
}

public static class MapExtensions
{
    //Dictionary with properly formatted map names
    //If map name is simple the method gets the ToString result
    private static readonly Dictionary<Map, string> names = new()
    {
        { Map.MarcinK, "Marcin K" },
    };

    public static string GetName(Map map)
    {
        if (names.TryGetValue(map, out string name))
            return name;
        return map.ToString();
    }
}
