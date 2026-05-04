namespace ExampleTemplate.WebApp.Tests.Setup.Authentication;

public static class TestAuthenticationExtension
{
    public static HttpClient ConfigureTestAuthentication(this HttpClient client)
    {
        var token = AuthenticationTokenGenerator.GenerateJwtToken(new Dictionary<string, string>()
        {
            { "aud", AuthenticationTokenGenerator.audience },
            { "iss", AuthenticationTokenGenerator.issuer }
        });
        client.DefaultRequestHeaders.Authorization = new($"Bearer", token);

        return client;
    }
}