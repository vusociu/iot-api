using System.ComponentModel.DataAnnotations;
using iot_project.Enum;

namespace iot_project.Models
{
    public class CheckCardHistory
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string idCard { get; set; }

        [Required]
        public CheckCardStatus status { get; set; }

        [Required]
        public DateTime time { get; set; }

        public string fullName { get; set; }
    }
}
