using SukkotStore.WebApp.Database.Models;

namespace SukkotStore.WebApp.Interfaces
{
    public interface IUserService
    {
        Task<UserEntity?> GetUserAsync(Guid id, CancellationToken cancellationToken);
    }
}