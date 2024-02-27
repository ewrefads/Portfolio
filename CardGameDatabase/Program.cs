using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameDatabase
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mapper = new CardGameMapper();
            var provider = new SQLiteDatabaseProvider("Data Source=cardgame.db; version=3;new=true");

            List<Card> result;
            var repo = new CardGameRepository(mapper, provider);
            while (true) {
                Console.Write("Enter command: ");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                else if (input == "clear") {
                    Console.Clear();
                }
                else if (input.Contains("SELECT"))
                {
                    repo.ExecuteReader(input);
                }
                else
                {
                    repo.ExecuteNonquery(input);
                }
            }
            
        }
    }
}
