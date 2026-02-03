using SubPayment.Models;

namespace SubPayment.Data
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id); 
        Task<User?> GetByUsernameAsync(string username); 
        Task AddAsync(User user); 
        Task UpdateAsync(User user);
    }
}

