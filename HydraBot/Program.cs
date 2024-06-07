using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using HydraBot.Services;
using HydraBot.Trackers;

namespace HydraBot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private IConfiguration _configuration;

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

            var databaseService = new DatabaseService(_configuration["DatabasePath"]);
            var repacksTrackerFactory = new RepacksTrackerFactory(_client, databaseService);
            var repacksTracker = repacksTrackerFactory.CreateRepacksTracker();

            await _client.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
            await _client.StartAsync();

            _client.MessageReceived += HandleCommandAsync;

            await repacksTracker.StartTracking(databaseService.GetNotificationChannel());

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            // Lógica para lidar com comandos do bot
        }
    }
}
