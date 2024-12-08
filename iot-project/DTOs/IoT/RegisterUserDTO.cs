namespace iot_project.DTOs.IoT
{
    public class RegisterUserDTO
    {
        public string fullName { get; set; }
        public DateTime birthday { get; set; } = DateTime.Parse("01/01/1970");
        public string phone { get; set; }  = "";
    }
}
