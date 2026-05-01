using Newtonsoft.Json;

namespace SukkotStore.WebApp.Models.Authentication;

public class SupaBaseAuthenticationResult
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public required long ExpiresIn { get; set; }

    [JsonProperty("expires_at")]
    public required long ExpiresAt { get; set; }
}
