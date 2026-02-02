using SubPayment.Models; 
using Microsoft.EntityFrameworkCore;

namespace SubPayment.Data
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public SqlUserRepository(UserDbContext context)
        {
            _context = context;
        }

        public User? GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User? GetByUsername(string username) 
        { 
            return _context.Users.SingleOrDefault(u => u.Username == username); 
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
