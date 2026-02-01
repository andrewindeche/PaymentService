using Microsoft.EntityFrameworkCore;

public class SqlUserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    public SqlUserRepository(UserDbContext context) 
    { 
        _context = context; 
    }

    public User GetById(string userId)
    {
        var user = _context.Users.Find(userId); 
        if (user == null) throw new KeyNotFoundException($"User {userId} not found"); 
        return user;
    }

    public void Update(User user)
    {
        _context.Users.Update(user); 
        _context.SaveChanges();
    }

    public void Add(User user) 
    { 
        _context.Users.Add(user); _context.SaveChanges(); 
    }
}
