using System.ComponentModel.DataAnnotations;
using iot_project.Enum;

namespace iot_project.DTOs.Authenticate
{
    public class RegisterDTO
    {
        [Key]
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public Role role { get; set; } = Role.USER;
    }
}
