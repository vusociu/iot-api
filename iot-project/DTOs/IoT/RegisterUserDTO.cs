namespace iot_project.DTOs.IoT
{
    public class RegisterUserDTO
    {
        public string fullName { get; set; }
        public string birthday { get; set; } = "01/01/1970";
        public string phone { get; set; }  = "";
    }
}
