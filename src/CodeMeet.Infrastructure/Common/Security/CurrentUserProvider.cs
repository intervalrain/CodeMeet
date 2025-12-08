using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using Microsoft.AspNetCore.Http;
using Throw;

namespace CodeMeet.Infrastructure.Common.Security;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUser CurrentUser => GetCurrentUser();

    private CurrentUser GetCurrentUser()
    {
        _httpContextAccessor.HttpContext.ThrowIfNull();

        var id = Guid.Parse(GetSingleClaimValue("id"));
        var permissions = GetClaimValues("permissions");
        var roles = GetClaimValues(ClaimTypes.Role);
        var username = GetSingleClaimValue(JwtRegisteredClaimNames.Name);
        var email = GetSingleClaimValue(ClaimTypes.Email);

        return new CurrentUser(id, username, email, permissions, roles);
    }

    private List<string> GetClaimValues(string claimType) =>
    _httpContextAccessor.HttpContext!.User.Claims
        .Where(claim => claim.Type == claimType)
        .Select(claim => claim.Value)
        .ToList();

    private string GetSingleClaimValue(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}