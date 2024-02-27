using CardGameDatabase.RepositoryPattern;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CardGameDatabase
{
    internal class CardGameRepository: ICardGameRepository
    {
        private CardGameMapper mapper;
        private SQLiteDatabaseProvider provider;

        public CardGameRepository(CardGameMapper mapper, SQLiteDatabaseProvider provider)
        {
            this.mapper = mapper;
            this.provider = provider;

            CreateDatabaseTables();
        }

        private void CreateDatabaseTables()
        {
            var connection = provider.CreateConnection();
            connection.Open();


            var cmd = new SQLiteCommand($"DROP TABLE users", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"DROP TABLE deck", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"DROP TABLE card_type", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"DROP TABLE card_instance", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"DROP TABLE card_shop", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"CREATE TABLE users(userid INTEGER PRIMARY KEY, username VARCHAR(50), password VARCHAR({int.MaxValue}));", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"CREATE TABLE deck(deckId INTEGER PRIMARY KEY, deckName VARCHAR(50), Card_Amount INTEGER, owner INTEGER, FOREIGN KEY(owner) REFERENCES users(userid));", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"CREATE TABLE card_type(card_type VARCHAR(50) PRIMARY KEY, card_image VARCHAR(50), health INTEGER, damage INTEGER, cost Integer,ability_1 VARCHAR(50), ability_2 VARCHAR(50));", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"CREATE TABLE card_instance(instance_id INTEGER PRIMARY KEY, amount INTEGER, card_type VARCHAR(50), owner INTEGER, deck INTEGER, FOREIGN KEY (card_type) REFERENCES card_type(card_type), FOREIGN KEY(deck) REFERENCES deck(deck_id), FOREIGN KEY(owner) REFERENCES user(userid));", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            cmd = new SQLiteCommand($"CREATE TABLE card_shop(card_type VARCHAR(50) PRIMARY KEY, price INTEGER, sale_percent, description VARCHAR(2400), FOREIGN KEY (card_type) references card_type(card_type));", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            

            connection.Close();
        }

        public void AddCard(string cardType, string imageLocation,int health, int damage, int cost, string ability1, string ability2)
        {
            var connection = provider.CreateConnection();
            connection.Open();

            
            var cmd = new SQLiteCommand($"INSERT INTO card_types(name, image, health, damage, cost, ability_1, ability_2) VALUES('{cardType}', '{imageLocation}',{health}, {damage}, {cost}, '{ability1}', '{ability2}');", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public Card FindCard(string cardType)
        {
            var connection = provider.CreateConnection();
            connection.Open();


            var cmd = new SQLiteCommand($"SELECT * FROM card_types WHERE name = '{cardType}';", (SQLiteConnection)connection);
            var data = cmd.ExecuteReader();

            Card card = new Card();
            
            while (data.Read())
            {

                card.Name = data.GetString(0);
                card.Health = data.GetInt32(1);
                card.Damage = data.GetInt32(2);
            }

            connection.Close();

            

            return card;
        }

        public void ExecuteNonquery(string command) {
            var connection = provider.CreateConnection();
            connection.Open();


            var cmd = new SQLiteCommand(command, (SQLiteConnection)connection);
            try
            {
                int res = cmd.ExecuteNonQuery();
                Console.WriteLine($"{res} rows affected");
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());

            }
            connection.Close();
        }

        public void ExecuteReader(string command)
        {
            var connection = provider.CreateConnection();
            connection.Open();
            try
            {
                var cmd = new SQLiteCommand(command, (SQLiteConnection)connection);
                var data = cmd.ExecuteReader();
                int l = data.FieldCount;
                string[] collumns = new string[l];

                for (int j = 0; j < l; j++)
                {
                    collumns[j] = $"{data.GetName(j)}";
                }
                var table = new ConsoleTable(collumns);

                while (data.Read())
                {

                    string[] row = new string[l];
                    for (int i = 0; i < l; i++)
                    {
                        row[i] = $"{data.GetFieldValue<object>(i)}";
                    }
                    table.AddRow(row);
                }
                table.Write();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
            

            connection.Close();
        }


        public List<Card> GetAllCards()
        {
            var connection = provider.CreateConnection();
            connection.Open();


            var cmd = new SQLiteCommand($"SELECT * FROM cards;", (SQLiteConnection)connection);
            var data = cmd.ExecuteReader();

            List<Card> result = new List<Card>();
            while (data.Read())
            {
                Card card = new Card();
                card.Name = data.GetString(0);
                card.Health = data.GetInt32(1);
                card.Damage = data.GetInt32(2);
                result.Add(card);
            }
            connection.Close();

            
            return result;
        }
    }
}