using CodeMeet.Application.Gamification;

namespace CodeMeet.Infrastructure.Gamification;

/// <summary>
/// Default implementation of <see cref="IGamificationService"/>.
/// Returns a fixed value until the Gamification domain is implemented.
/// </summary>
public class GamificationService : IGamificationService
{
    private const int DefaultOpportunities = 1;

    public Task<int> GetOpportunitiesAsync(Guid userId, CancellationToken token = default)
    {
        // TODO: Implement actual opportunity lookup from Gamification domain
        return Task.FromResult(DefaultOpportunities);
    }
}
