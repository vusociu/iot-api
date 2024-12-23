using iot_project.Models;

namespace iot_project.Transformer
{
    public class ListCheckCardHistoryTransformer
    {
        public List<CheckCardHistoryTransformer> transformFromList(List<CheckCardHistory> list)
        {
            var transformers = new List<CheckCardHistoryTransformer>();
            foreach (var history in list)
            {
                transformers.Add(new CheckCardHistoryTransformer(history));
            }
            return transformers;
        }
    }
}
