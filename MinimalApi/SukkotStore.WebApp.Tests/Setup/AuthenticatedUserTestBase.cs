namespace SukkotStore.WebApp.Tests.Setup;

public abstract class AuthenticatedUserTestBase
    : IntegrationTestBase
{
    public AuthenticatedUserTestBase(Guid userId) : base(new TestUserContext()
    {
        Email = "fakeUser@gmail.com",
        UserId = userId
    })
    { }

    public AuthenticatedUserTestBase(string userEmail, Guid userId) : base(new TestUserContext()
    {
        Email = userEmail,
        UserId = userId
    })
    { }
}
