using CodeMeet.Application.Gamification;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Gamification.Entities;

namespace CodeMeet.Infrastructure.Gamification;

/// <summary>
/// Implementation of <see cref="IGamificationService"/> that manages user opportunities.
/// Automatically creates UserOpportunity for new users with initial balance of 1.
/// </summary>
public class GamificationService(IRepository<UserOpportunity> repository) : IGamificationService
{
    public async Task<int> GetOpportunitiesAsync(Guid userId, CancellationToken ct = default)
    {
        var opportunity = await GetOrCreateOpportunityAsync(userId, ct);
        return opportunity.Balance;
    }

    public async Task<bool> HasOpportunityAsync(Guid userId, CancellationToken ct = default)
    {
        var opportunity = await GetOrCreateOpportunityAsync(userId, ct);
        return opportunity.HasOpportunity();
    }

    public async Task<bool> TryConsumeAsync(Guid userId, CancellationToken ct = default)
    {
        var opportunity = await GetOrCreateOpportunityAsync(userId, ct);

        if (!opportunity.TryConsume())
        {
            return false;
        }

        await repository.UpdateAsync(opportunity, ct);
        return true;
    }

    public async Task AwardAsync(Guid userId, int amount = 1, CancellationToken ct = default)
    {
        var opportunity = await GetOrCreateOpportunityAsync(userId, ct);
        opportunity.Award(amount);

        await repository.UpdateAsync(opportunity, ct);
    }

    public async Task<bool> TryAwardDailyAsync(Guid userId, CancellationToken ct = default)
    {
        var opportunity = await GetOrCreateOpportunityAsync(userId, ct);

        if (!opportunity.TryAwardDaily())
        {
            return false;
        }

        await repository.UpdateAsync(opportunity, ct);
        return true;
    }

    private async Task<UserOpportunity> GetOrCreateOpportunityAsync(Guid userId, CancellationToken ct)
    {
        var opportunity = await repository.FindAsync(o => o.UserId == userId, ct);

        if (opportunity is not null)
        {
            return opportunity;
        }

        // Create new UserOpportunity with initial balance of 1
        opportunity = UserOpportunity.Create(userId);
        await repository.InsertAsync(opportunity, ct);

        return opportunity;
    }
}
