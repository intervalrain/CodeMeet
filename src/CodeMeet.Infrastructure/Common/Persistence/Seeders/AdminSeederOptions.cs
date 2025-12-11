namespace CodeMeet.Infrastructure.Common.Persistence.Seeders;

public class AdminSeederOptions
{
    public const string SectionName = "AdminSeeder";

    public string Username { get; set; } = "admin";
    public string Email { get; set; } = "admin@codemeet.dev";
    public string Password { get; set; } = string.Empty;
}