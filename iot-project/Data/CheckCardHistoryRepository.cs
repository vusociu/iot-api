using iot_project.Enum;
using iot_project.Models;

namespace iot_project.Data
{
    public class CheckCardHistoryRepository : ICheckCardHistoryRepository
    {
        private readonly AppDbContext _context;

        public CheckCardHistoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public CheckCardHistory create(CheckCardHistory checkCardHistory)
        {
            _context.CheckCardHistories.Add(checkCardHistory);
            checkCardHistory.id = _context.SaveChanges();
            return checkCardHistory;
        }

        public List<CheckCardHistory> listOpen()
        {
            return _context.CheckCardHistories.Where(u => u.status==CheckCardStatus.EXIST).ToList();
        }
    }
}
