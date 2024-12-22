using System.Linq;
using iot_project.Helpers;
using iot_project.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iot_project.Data
{
    public class IdentityCardRepository : IIdentityCardRepository
    {

        private readonly ILogger<IdentityCardRepository> _logger;
        private readonly AppDbContext _context;

        public IdentityCardRepository(
            AppDbContext context,
                        ILogger<IdentityCardRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }
        public IdentityCard create(IdentityCard identityCard)
        {
            try
            {
                _context.IdentityCards.Add(identityCard);
                _context.SaveChanges();
                return identityCard;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update database. Inner exception: {InnerException}", ex.InnerException?.Message);
                throw;
            }
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
