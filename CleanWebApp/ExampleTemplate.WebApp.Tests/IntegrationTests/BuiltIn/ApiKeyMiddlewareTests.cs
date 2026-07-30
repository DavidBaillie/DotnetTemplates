#if requireApiKey
using ExampleTemplate.WebApp.Tests.Setup;
using Shouldly;
using System.Net;

namespace ExampleTemplate.WebApp.Tests.IntegrationTests.BuiltIn;

/// <summary>
/// Integration tests for <see cref="ExampleTemplate.WebApp.Middleware.ApiKeyMiddleware"/>.
/// Validates API key authentication requirements for all endpoints.
/// </summary>
public sealed class ApiKeyMiddlewareTests
    : IntegrationTestBase
{
    private const string API_KEY_HEADER_NAME = "x-api-key";
    private const string TEST_ENDPOINT = "/health";
    private const string VALID_API_KEY = "test-api-key-12345";

    [Test]
    public async Task Invoke_WithValidApiKey_ReturnsSuccess()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        request.Headers.Add(API_KEY_HEADER_NAME, VALID_API_KEY);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task Invoke_WithMissingApiKeyHeader_ReturnsUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        // Deliberately not adding the API key header

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Invoke_WithEmptyApiKeyValue_ReturnsUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        request.Headers.Add(API_KEY_HEADER_NAME, string.Empty);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Invoke_WithInvalidApiKey_ReturnsUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        request.Headers.Add(API_KEY_HEADER_NAME, "invalid-key");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Invoke_WithIncorrectCasing_ReturnsUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        request.Headers.Add(API_KEY_HEADER_NAME, VALID_API_KEY.ToUpperInvariant());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Invoke_WithDifferentLengthKey_ReturnsUnauthorized()
    {
        // Arrange - Tests constant-time comparison with different length
        using var request = new HttpRequestMessage(HttpMethod.Get, TEST_ENDPOINT);
        request.Headers.Add(API_KEY_HEADER_NAME, VALID_API_KEY + "extra");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
#endif
