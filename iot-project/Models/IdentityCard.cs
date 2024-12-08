using System.ComponentModel.DataAnnotations;

namespace iot_project.Models
{
    public class IdentityCard
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string idCard { get; set; }

        [Required]
        public string fullName { get; set; }

        public DateTime birthday { get; set; }
        public string phone { get; set; }
    }
}
