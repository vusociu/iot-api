using iot_project.Models;

namespace iot_project.Data
{
    public interface ICheckCardHistoryRepository
    {
        CheckCardHistory create(CheckCardHistory checkCardHistory);
        List<CheckCardHistory> listOpen();
    }
}
