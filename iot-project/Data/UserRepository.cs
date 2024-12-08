using iot_project.Enum;
using iot_project.Models;
using System;

namespace iot_project.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public User create(User user)
        {
            _context.Users.Add(user);
            user.id = _context.SaveChanges();
            return user;
        }
        public User getByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.email == email);
        }

        public User getAdminByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.email == email && u.role==Role.ADMIN);
        }

        public User getById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.id == id);
        }
    }
}
