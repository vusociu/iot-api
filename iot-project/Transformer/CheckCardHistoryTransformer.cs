using iot_project.Models;

namespace iot_project.Transformer
{
    public class CheckCardHistoryTransformer
    {
        public string fullName { get; set; }
        public string idCard { get; set; }
        public string openAt { get; set; }

        public CheckCardHistoryTransformer(CheckCardHistory history)
        {
            this.fullName = history.fullName ?? "";
            this.idCard = history.idCard;
            this.openAt = history.time.ToString("HH:mm:ss dd-MM-yyyy");
        }
    }
}
