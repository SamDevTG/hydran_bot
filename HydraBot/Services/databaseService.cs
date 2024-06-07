using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;
using HydraBot.Models;

namespace HydraBot.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string databasePath)
        {
            _connectionString = $"Data Source={databasePath}";
        }

        public IEnumerable<Repack> GetExistingRepacks()
        {
            using var connection = new SQLiteConnection(_connectionString);
            return connection.Query<Repack>("SELECT * FROM repacks");
        }

        public void SaveRepacks(IEnumerable<Repack> repacks)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Execute("INSERT INTO repacks (Title, Magnet, UploadDate, Repacker) VALUES (@Title, @Magnet, @UploadDate, @Repacker)", repacks);
        }
    }
}
