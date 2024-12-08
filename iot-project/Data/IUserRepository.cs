using iot_project.Models;

namespace iot_project.Data
{
    public interface IUserRepository
    {
        User create(User user);
        User getByEmail(string email);
        User getAdminByEmail(string email);
        User getById(int id);
    }
}
