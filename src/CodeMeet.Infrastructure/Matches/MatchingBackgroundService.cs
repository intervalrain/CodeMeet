using CodeMeet.Application.Gamification;
using CodeMeet.Application.Matches;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Matches.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeMeet.Infrastructure.Matches;

/// <summary>
/// Background service that periodically processes the match queue
/// and creates matches for compatible pairs.
/// </summary>
public class MatchingBackgroundService(
    IServiceProvider serviceProvider,
    IMatchQueueService queueService,
    ILogger<MatchingBackgroundService> logger)
    : BackgroundService
{
    private readonly TimeSpan _matchingInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Matching background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMatchesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in matching background service");
            }

            await Task.Delay(_matchingInterval, stoppingToken);
        }

        logger.LogInformation("Matching background service stopped");
    }

    private async Task ProcessMatchesAsync(CancellationToken ct)
    {
        var pairs = await queueService.FindCompatiblePairsAsync(ct);

        if (pairs.Count == 0)
        {
            return;
        }

        logger.LogInformation("Found {Count} compatible pairs to match", pairs.Count);

        foreach (var pair in pairs)
        {
            await ProcessSingleMatchAsync(pair, ct);
        }
    }

    private async Task ProcessSingleMatchAsync(MatchPair pair, CancellationToken ct)
    {
        // Create a scope for scoped services (repository, unit of work)
        using var scope = serviceProvider.CreateScope();
        var gamificationService = scope.ServiceProvider.GetRequiredService<IGamificationService>();
        var matchRepository = scope.ServiceProvider.GetRequiredService<IRepository<Match>>();
        var notificationService = scope.ServiceProvider.GetRequiredService<IMatchNotificationService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            // 1. Try to consume opportunity from interviewee
            var consumed = await gamificationService.TryConsumeAsync(pair.Interviewee.UserId, ct);

            if (!consumed)
            {
                logger.LogInformation(
                    "User {UserId} has insufficient opportunities, removing from queue",
                    pair.Interviewee.UserId);

                await queueService.DequeueAsync(pair.Interviewee.UserId, ct);
                await notificationService.NotifyInsufficientOpportunitiesAsync(pair.Interviewee.UserId, ct);
                return;
            }

            // 2. Create Match entity
            var match = Match.Create(
                intervieweeId: pair.Interviewee.UserId,
                interviewerId: pair.Interviewer.UserId,
                difficulty: pair.CommonDifficulty,
                enableVideo: pair.Interviewee.EnableVideo,
                suggestedQuestionId: null); // TODO: Integrate with question bank

            // 3. Generate placeholder URLs
            var documentUrl = $"https://codemeet.app/doc/{match.Id}";
            var videoRoomUrl = pair.Interviewee.EnableVideo
                ? $"https://codemeet.app/video/{match.Id}"
                : null;

            match.MarkReady(documentUrl, videoRoomUrl);

            // 4. Persist match
            await matchRepository.InsertAsync(match, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // 5. Remove both users from queue
            await queueService.RemovePairAsync(
                pair.Interviewee.UserId,
                pair.Interviewer.UserId,
                ct);

            // 6. Notify both users
            await notificationService.NotifyMatchFoundAsync(
                match.Id,
                pair.Interviewee.UserId,
                pair.Interviewer.UserId,
                ct);

            await notificationService.NotifyMatchReadyAsync(
                pair.Interviewee.UserId,
                match.Id,
                documentUrl,
                videoRoomUrl,
                ct);

            await notificationService.NotifyMatchReadyAsync(
                pair.Interviewer.UserId,
                match.Id,
                documentUrl,
                videoRoomUrl,
                ct);

            logger.LogInformation(
                "Match created: {MatchId} - Interviewee: {IntervieweeId}, Interviewer: {InterviewerId}",
                match.Id, pair.Interviewee.UserId, pair.Interviewer.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to process match for users {IntervieweeId} and {InterviewerId}",
                pair.Interviewee.UserId, pair.Interviewer.UserId);

            // Refund the opportunity if it was consumed
            // Note: This is a best-effort rollback
            try
            {
                await gamificationService.AwardAsync(pair.Interviewee.UserId, 1, ct);
            }
            catch (Exception refundEx)
            {
                logger.LogError(refundEx,
                    "Failed to refund opportunity to user {UserId}",
                    pair.Interviewee.UserId);
            }
        }
    }
}
