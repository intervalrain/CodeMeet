using System.Runtime.Serialization;

namespace CodeMeet.Domain.Matches.Enums;

[Flags]
public enum Difficulty
{
    [EnumMember(Value = "easy")]
    Easy = 1,

    [EnumMember(Value = "medium")]
    Medium = 2,
    
    [EnumMember(Value = "hard")]
    Hard = 4,
 
    [EnumMember(Value = "both")]
    Both = Easy | Medium | Hard
}