using CardGameDatabase.RepositoryPattern;
using System;
using System.Data;
using System.Data.SQLite;

namespace CardGameDatabase
{
    internal class SQLiteDatabaseProvider: IDatabaseProvider
    {
        private string connectionString;

        public SQLiteDatabaseProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}