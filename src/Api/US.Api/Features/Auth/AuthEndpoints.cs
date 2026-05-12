using Microsoft.AspNetCore.Mvc;
using US.SharedKernel.Contracts.Auth;

namespace US.Api.Features.Auth;

public static class AuthEndpoints
{
    private static readonly IReadOnlyList<SeedUser> Users =
    [
        new("validator", "Validator123!", "Amina Validator", "Validator"),
        new("admin", "Admin123!", "Ubuntu Admin", "Admin")
    ];

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", ([FromBody] LoginRequest request) =>
        {
            var user = Users.FirstOrDefault(candidate =>
                string.Equals(candidate.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase) &&
                candidate.Password == request.Password);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new LoginResponse
            {
                Username = user.Username,
                DisplayName = user.DisplayName,
                Role = user.Role
            });
        });

        group.MapGet("/demo-users", () => Results.Ok(Users.Select(user => new
        {
            user.Username,
            user.DisplayName,
            user.Role
        })));

        return endpoints;
    }

    private sealed record SeedUser(string Username, string Password, string DisplayName, string Role);
}
