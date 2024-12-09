using iot_project.Models;

namespace iot_project.Transformer
{
    public class ListCheckCardHistoryTransformer
    {
        public List<CheckCardHistoryTransformer> transformFromList(List<CheckCardHistory> list, Dictionary<string, IdentityCard> listCard)
        {
            var transformers = new List<CheckCardHistoryTransformer>();
            foreach (var history in list)
            {
                string fullName = "";
                if (listCard.TryGetValue(history.idCard, out IdentityCard identityCard))
                {
                    fullName = identityCard.fullName;
                }
                transformers.Add(new CheckCardHistoryTransformer(
                    fullName, history.idCard, history.time.ToString("HH:mm:ss dd-MM-yyyy")
                ));
            }
            return transformers;
        }
    }
}
