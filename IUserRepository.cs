using SubPayment.Models;

public interface IUserRepository
{
    User? GetById(int id);               
    User? GetByUsername(string username);
    void Add(User user);
    void Update(User user);
}
