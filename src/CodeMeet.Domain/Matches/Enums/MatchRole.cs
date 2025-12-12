using System.Runtime.Serialization;

namespace CodeMeet.Domain.Matches.Enums;

[Flags]
public enum MatchRole
{
    [EnumMember(Value = "interviewee")]
    Interviewee = 1,

    [EnumMember(Value = "interviewer")]
    Interviewer = 2,

    [EnumMember(Value = "both")]
    Both = Interviewee | Interviewer
}