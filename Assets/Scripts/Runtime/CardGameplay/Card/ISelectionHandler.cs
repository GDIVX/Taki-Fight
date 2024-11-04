using System.Threading.Tasks;

namespace Runtime.CardGameplay.Card
{
    public interface ISelectionHandler
    {
        Task<bool> SelectAsync(CardController card);
    }
}