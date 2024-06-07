using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace DiscordBot
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

    public class Repack
    {
        public string Title { get; set; }
        public string Magnet { get; set; }
        public DateTime UploadDate { get; set; }
        public string Repacker { get; set; }
    }
}
