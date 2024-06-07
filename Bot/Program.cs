using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SQLite;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text;
using System.HydraBot.Models

namespace DiscordBot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private IConfiguration _configuration;
        private DatabaseService _databaseService;
        private ulong _notificationChannelId;

        static Task Main(string[] args) => new Program().MainAsync();

        public Program()
        {
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
        }

        public async Task MainAsync()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _databaseService = new DatabaseService(_configuration["DatabasePath"]);

            _notificationChannelId = _databaseService.GetNotificationChannel();

            await _client.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
            await _client.StartAsync();

            _client.MessageReceived += HandleCommandAsync;

            MonitorRepacks();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            var content = message.Content;

            if (content.StartsWith(_configuration["CommandPrefix"]))
            {
                var command = content.Substring(1);

                if (command.StartsWith("setchannel"))
                {
                    var channel = message.MentionedChannels.FirstOrDefault();
                    if (channel != null)
                    {
                        _notificationChannelId = channel.Id;
                        _databaseService.SaveNotificationChannel(_notificationChannelId);
                        await message.Channel.SendMessageAsync($"Canal de notificação definido para {channel.Name}");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Por favor, mencione um canal válido.");
                    }
                }
            }
        }

        private async void MonitorRepacks()
        {
            while (true)
            {
                var existingRepacks = _databaseService.GetExistingRepacks();
                var newRepacks = await GetNewRepacks(existingRepacks);

                if (newRepacks.Any() && _notificationChannelId != 0)
                {
                    var channel = _client.GetChannel(_notificationChannelId) as IMessageChannel;
                    foreach (var repack in newRepacks)
                    {
                        await channel.SendMessageAsync($"Novo repack disponível: {repack.Title} - {repack.Magnet}");
                    }

                    _databaseService.SaveRepacks(newRepacks);
                }

                await Task.Delay(TimeSpan.FromHours(1));
            }
        }

        private async Task<IEnumerable<Repack>> GetNewRepacks(IEnumerable<Repack> existingRepacks)
        {
            List<Repack> newRepacks = new List<Repack>();
            var repacksFromHydra = await GetRepacksFromHydra();

            foreach (var repack in repacksFromHydra)
            {
                if (!existingRepacks.Any(existingRepack => existingRepack.Title == repack.Title))
                {
                    newRepacks.Add(repack);
                }
            }

            return newRepacks;
        }

        private async Task<IEnumerable<Repack>> GetRepacksFromHydra()
        {
            List<Repack> repacks = new List<Repack>();
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync("https://freegogpcgames.com/a-z-games-list/");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var gamesList = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'az-columns')]/ul/li/a");

            foreach (var game in gamesList)
            {
                var href = game.GetAttributeValue("href", "");
                var title = game.InnerText.Trim();
                var gameExists = repacks.Any(existingRepack => existingRepack.Title == title);

                if (!gameExists)
                {
                    var gogGame = await GetGOGGame(href);

                    if (gogGame != null)
                    {
                        repacks.Add(new Repack
                        {
                            Title = title,
                            FileSize = gogGame.FileSize ?? "N/A",
                            UploadDate = gogGame.UploadDate,
                            Repacker = "GOG",
                            Magnet = GetMagnet(gogGame.DownloadLink),
                            Page = 1
                        });
                    }
                }
            }

            return repacks;
        }

        private async Task<GOGGame> GetGOGGame(string url)
        {
            // Lógica para obter informações do jogo GOG
            return null;
        }

        private string GetMagnet(string downloadLink)
        {
            // Lógica para obter o magnet link
            return null;
        }
    }

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

        public ulong GetNotificationChannel()
        {
            using var connection = new SQLiteConnection(_connectionString);
            return connection.QuerySingleOrDefault<ulong>("SELECT ChannelId FROM NotificationChannel LIMIT 1");
        }

        public void SaveNotificationChannel(ulong channelId)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Execute("REPLACE INTO NotificationChannel (Id, ChannelId) VALUES (1, @ChannelId)", new { ChannelId = channelId });
        }
    }
}
