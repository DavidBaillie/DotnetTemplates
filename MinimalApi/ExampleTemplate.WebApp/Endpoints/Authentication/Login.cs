using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SukkotStore.WebApp.Constants;
using SukkotStore.WebApp.Models.Authentication;

namespace SukkotStore.WebApp.Endpoints.Authentication;

public static class Login
{
    public static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken)
    {
        if (RequestIsInvalid(request))
            return TypedResults.Unauthorized();

        using var client = httpClientFactory.CreateClient(HttpClientNames.AuthenticationClient);
        using var response = await client.PostAsJsonAsync(
            "auth/v1/token?grant_type=password",
            new
            {
                email = request.Email,
                password = request.Password,
            }, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return TypedResults.Unauthorized();

        var content = JsonConvert.DeserializeObject<SupaBaseAuthenticationResult>(
            await response.Content.ReadAsStringAsync(cancellationToken));

        if (content is null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(new LoginResponse()
        {
            AccessToken = content.AccessToken,
            RefreshToken = content.RefreshToken,
            ExpiresIn = content.ExpiresIn,
        });
    }

    private static bool RequestIsInvalid(LoginRequest request)
    {
        return request is null ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Email.Length > 256 ||
            request.Password.Length > 256;
    }

    public sealed class LoginRequest
    {
        /// <summary>
        /// Email to login with
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password for the user account
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    public sealed class LoginResponse
    {
        /// <summary>
        /// Access token to use with future requests
        /// </summary>
        public required string AccessToken { get; set; }

        /// <summary>
        /// Refresh token to be used when the previous token expires
        /// </summary>
        public required string RefreshToken { get; set; }

        /// <summary>
        /// How many seconds until the token expires
        /// </summary>
        public required long ExpiresIn { get; set; }
    }
}
