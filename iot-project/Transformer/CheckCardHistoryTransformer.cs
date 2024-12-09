namespace iot_project.Transformer
{
    public class CheckCardHistoryTransformer
    {
        public string fullName { get; set; }
        public string idCard { get; set; }
        public string openAt { get; set; }

        public CheckCardHistoryTransformer(string fullName, string idCard, string openAt)
        {
            this.fullName = fullName;
            this.idCard = idCard;
            this.openAt = openAt;
        }
    }
}
