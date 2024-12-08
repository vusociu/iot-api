using System.ComponentModel.DataAnnotations;
using iot_project.Enum;

namespace iot_project.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string fullName { get; set; }
        public Role role { get; set; }
    }
}
