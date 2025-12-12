using System.Text.Json.Serialization;
using CodeMeet.Ddd.Domain;
using CodeMeet.Domain.Matches.Events;
using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Domain.Matches.Entities;

/// <summary>
/// Represents a successful match between two users for an interview session.
/// This is persisted to track interview history.
/// </summary>
public class Match : AggregationRoot
{
    public Guid IntervieweeId { get; private set; }
    public Guid InterviewerId { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public bool EnableVideo { get; private set; }
    public MatchStatus Status { get; private set; }
    public string DocumentUrl { get; private set; } = string.Empty;
    public string? VideoRoomUrl { get; private set; }

    /// <summary>
    /// Suggested question ID from question bank (e.g., LeetCode problem number).
    /// Null if no question was suggested. Interviewer may choose to use it or not.
    /// </summary>
    public int? SuggestedQuestionId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadyAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private Match() { } // EF

    [JsonConstructor]
    private Match(
        Guid intervieweeId,
        Guid interviewerId,
        Difficulty difficulty,
        bool enableVideo,
        MatchStatus status,
        string documentUrl,
        string? videoRoomUrl,
        int? suggestedQuestionId,
        DateTime createdAt,
        DateTime? readyAt,
        DateTime? completedAt)
    {
        IntervieweeId = intervieweeId;
        InterviewerId = interviewerId;
        Difficulty = difficulty;
        EnableVideo = enableVideo;
        Status = status;
        DocumentUrl = documentUrl;
        VideoRoomUrl = videoRoomUrl;
        SuggestedQuestionId = suggestedQuestionId;
        CreatedAt = createdAt;
        ReadyAt = readyAt;
        CompletedAt = completedAt;
    }

    /// <summary>
    /// Creates a new match between two users.
    /// </summary>
    /// <param name="suggestedQuestionId">Optional question ID from question bank (e.g., LeetCode number)</param>
    public static Match Create(
        Guid intervieweeId,
        Guid interviewerId,
        Difficulty difficulty,
        bool enableVideo,
        int? suggestedQuestionId = null)
    {
        if (intervieweeId == Guid.Empty)
            throw new ArgumentException("Interviewee ID cannot be empty", nameof(intervieweeId));

        if (interviewerId == Guid.Empty)
            throw new ArgumentException("Interviewer ID cannot be empty", nameof(interviewerId));

        if (intervieweeId == interviewerId)
            throw new ArgumentException("Interviewee and interviewer cannot be the same user");

        var match = new Match
        {
            IntervieweeId = intervieweeId,
            InterviewerId = interviewerId,
            Difficulty = difficulty,
            EnableVideo = enableVideo,
            SuggestedQuestionId = suggestedQuestionId,
            Status = MatchStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        match.AddDomainEvent(new MatchCreatedEvent(
            match.Id,
            intervieweeId,
            interviewerId,
            difficulty,
            enableVideo,
            suggestedQuestionId));

        return match;
    }

    /// <summary>
    /// Marks the match as ready with assigned resources.
    /// </summary>
    public void MarkReady(string documentUrl, string? videoRoomUrl)
    {
        if (Status != MatchStatus.Pending)
            throw new InvalidOperationException($"Cannot mark match as ready with status {Status}");

        if (string.IsNullOrWhiteSpace(documentUrl))
            throw new ArgumentException("Document URL cannot be empty", nameof(documentUrl));

        if (EnableVideo && string.IsNullOrWhiteSpace(videoRoomUrl))
            throw new ArgumentException("Video room URL is required when video is enabled", nameof(videoRoomUrl));

        DocumentUrl = documentUrl;
        VideoRoomUrl = videoRoomUrl;
        Status = MatchStatus.Ready;
        ReadyAt = DateTime.UtcNow;

        AddDomainEvent(new MatchReadyEvent(Id, DocumentUrl, VideoRoomUrl));
    }

    /// <summary>
    /// Marks the match as completed.
    /// </summary>
    public void Complete()
    {
        if (Status != MatchStatus.Ready)
            throw new InvalidOperationException($"Cannot complete match with status {Status}");

        Status = MatchStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the match.
    /// </summary>
    public void Cancel()
    {
        if (Status == MatchStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed match");

        Status = MatchStatus.Canceled;
    }

    /// <summary>
    /// Checks if a user is part of this match.
    /// </summary>
    public bool IsParticipant(Guid userId)
    {
        return IntervieweeId == userId || InterviewerId == userId;
    }

    /// <summary>
    /// Gets the role of a user in this match.
    /// </summary>
    public MatchRole? GetUserRole(Guid userId)
    {
        if (IntervieweeId == userId) return MatchRole.Interviewee;
        if (InterviewerId == userId) return MatchRole.Interviewer;
        return null;
    }
}
