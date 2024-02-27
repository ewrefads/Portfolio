using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameDatabase.RepositoryPattern
{
    internal interface ICardGameRepository
    {
        void AddCard(string cardType, string imageLocation, int health, int damage, int cost, string ability1, string ability2);

        Card FindCard(string name);

        List<Card> GetAllCards();
    }
}
