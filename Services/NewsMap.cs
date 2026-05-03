namespace HindawiFoundation.Web.Services;

public static class NewsMap
{
    private static readonly string[] IndexToId =
    [
        "31794631",
        "38989894",
        "68390398",
        "75670183",
        "78235459",
        "95621490",
    ];

    private static readonly Dictionary<string, int> IdToIndex = new(StringComparer.Ordinal)
    {
        { "31794631", 0 },
        { "38989894", 1 },
        { "68390398", 2 },
        { "75670183", 3 },
        { "78235459", 4 },
        { "95621490", 5 },
    };

    public static string GetId(int index) =>
        index >= 0 && index < IndexToId.Length ? IndexToId[index] : IndexToId[0];

    public static int GetIndex(string id) =>
        IdToIndex.TryGetValue(id, out var index) ? index : 0;

    public static bool IsValid(string? id) =>
        !string.IsNullOrEmpty(id) && IdToIndex.ContainsKey(id);

    public static string Resolve(string? id) =>
        IsValid(id) ? id! : IndexToId[0];

    public static int Count => IndexToId.Length;
}
