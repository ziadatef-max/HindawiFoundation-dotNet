namespace HindawiFoundation.Web.Services;

/// <summary>
/// Maps sequential news indices (0–5) to their public 8-digit IDs used in URLs.
/// </summary>
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

    /// <summary>Returns the public ID for a 0-based index. Falls back to the first ID.</summary>
    public static string GetId(int index) =>
        index >= 0 && index < IndexToId.Length ? IndexToId[index] : IndexToId[0];

    /// <summary>Returns the 0-based index for a public ID. Falls back to 0.</summary>
    public static int GetIndex(string id) =>
        IdToIndex.TryGetValue(id, out var index) ? index : 0;

    /// <summary>Returns true if the given ID is a valid mapped news ID.</summary>
    public static bool IsValid(string? id) =>
        !string.IsNullOrEmpty(id) && IdToIndex.ContainsKey(id);

    /// <summary>Resolves an incoming ID to a valid one, defaulting to 11111111.</summary>
    public static string Resolve(string? id) =>
        IsValid(id) ? id! : IndexToId[0];

    public static int Count => IndexToId.Length;
}
