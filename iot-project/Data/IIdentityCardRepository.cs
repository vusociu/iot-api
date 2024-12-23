using iot_project.Models;

namespace iot_project.Data
{
    public interface IIdentityCardRepository
    {
        IdentityCard getByIdCard(string idCard);
        IdentityCard create(IdentityCard identityCard);
        void updateIdCard(int id, IdentityCard updateCard);
        List<IdentityCard> listByIdCards(string[] idsCards);

        public Dictionary<string, IdentityCard> keyById(List<IdentityCard> listCard);
    }
}
