namespace CodeMeet.Domain.Matches.Enums;

public enum MatchStatus
{
    /// <summary>
    /// Match created, awaiting resource assignment.
    /// </summary>
    Pending,

    /// <summary>
    /// Resources assigned, ready for interview to begin.
    /// </summary>
    Ready,

    /// <summary>
    /// Interview completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Match was canceled by user or system.
    /// </summary>
    Canceled
}