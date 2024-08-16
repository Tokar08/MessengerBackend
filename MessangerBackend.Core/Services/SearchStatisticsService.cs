namespace MessangerBackend.Core.Services;

public class SearchStatisticsService
{
    private readonly Dictionary<string, int> _searchStats = new();

    public void IncrementSearchStat(string searchTerm)
    {
        _searchStats[searchTerm] = _searchStats.TryGetValue(searchTerm, out int count) ? count + 1 : 1;
    }

    public IReadOnlyDictionary<string, int> GetStatistics() => _searchStats;
}
