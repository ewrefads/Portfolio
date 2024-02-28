using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin_Server
{
    

    public class LoginHandler
    {
        private static LoginHandler instance;

        private readonly string connectionString;


        private LoginHandler()
        {
            string databaseFileName = "accounts.db";
            connectionString = "Data Source=accounts.db";

            // Check if the database file exists, and create it if it doesn't
            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName);
            }


            // Create the accounts table if it doesn't exist
            CreateAccountsTable();
        }

        public static LoginHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginHandler();
                }
                return instance;
            }
        }

        public bool CreateAccount(string username, string password)
        {
            if (AccountExists(username))
            {
                Console.WriteLine("Username already exists. Please choose a different one.");
                return false;
            }

            // Generate a random salt
            byte[] salt = GenerateSalt();

            // Hash the password with the salt
            string hashedPassword = HashPassword(password, salt);

            // Create an Account object
            var account = new Account
            {
                username = username,
                password = hashedPassword,
                passwordSalt = salt,
                creationDate = DateTime.Now
            };

            // Insert the account into the database
            InsertAccount(account);

            Console.WriteLine("Account created successfully!");
            return true;
        }

        public bool Login(string username, string password, Guid user)
        {
            var account = GetAccountByUsername(username);

            if (account == null)
            {
                Console.WriteLine($"To server: {user} - {username} does not exist.");
                return false;
            }

            


                // Hash the provided password with the stored salt and check if it matches the stored hashed password
                return VerifyPassword(password, account.password, account.passwordSalt);
            
        }

        private bool AccountExists(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM Accounts WHERE username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); // 256 bits
                byte[] hashBytes = new byte[48]; // 32 bytes for hash + 16 bytes for salt

                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword, byte[] salt)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            string inputHashedPassword = HashPassword(password, salt);

            Console.WriteLine($"(SERVER) Encryption demo: Input password {inputHashedPassword}, Account password {hashedPassword}");

            return inputHashedPassword == hashedPassword;
        }

        private void CreateAccountsTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Accounts (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "username TEXT NOT NULL UNIQUE, " +
                    "password TEXT NOT NULL, " +
                    "passwordSalt BLOB NOT NULL, " +
                    "creationDate DATETIME NOT NULL)", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertAccount(Account account)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(
                    "INSERT INTO Accounts (username, password, passwordSalt, creationDate) " +
                    "VALUES (@username, @password, @passwordSalt, @creationDate)", connection))
                {
                    command.Parameters.AddWithValue("@username", account.username);
                    command.Parameters.AddWithValue("@password", account.password);
                    command.Parameters.AddWithValue("@passwordSalt", account.passwordSalt);
                    command.Parameters.AddWithValue("@creationDate", account.creationDate);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Account GetAccountByUsername(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Accounts WHERE username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var account = new Account
                            {
                                Id = reader.GetInt32(0),
                                username = reader.GetString(1),
                                password = reader.GetString(2),
                                passwordSalt = (byte[])reader[3],
                                creationDate = reader.GetDateTime(4)
                            };
                            return account;
                        }
                    }
                }
            }
            return null;


        }

        public void TestMethod()
        {
            Console.WriteLine("Loginhandler class - Test");
        }
    }
}
