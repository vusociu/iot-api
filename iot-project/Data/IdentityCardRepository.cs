using System.Linq;
using iot_project.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iot_project.Data
{
    public class IdentityCardRepository : IIdentityCardRepository
    {

        private readonly AppDbContext _context;

        public IdentityCardRepository(AppDbContext context)
        {
            _context = context;
        }
        public IdentityCard create(IdentityCard identityCard)
        {
            _context.IdentityCards.Add(identityCard);
            _context.SaveChanges();
            return identityCard;
        }

        public IdentityCard getByIdCard(string idCard)
        {
            return _context.IdentityCards.FirstOrDefault(u => u.idCard == idCard);
        }

        public List<IdentityCard> listByIdCards(string[] idsCards)
        {
            return _context.IdentityCards
                   .Where(u => idsCards.Contains(u.idCard))
                   .ToList();
        }

        public Dictionary<string, IdentityCard> keyById(List<IdentityCard> listCard)
        {
            var dictionary = listCard.ToDictionary(card => card.idCard);
            return dictionary;
        }
    }
}
