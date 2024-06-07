using Discord.WebSocket;
using HydraBot.Services;

namespace HydraBot.Trackers
{
    public class RepacksTrackerFactory
    {
        private readonly DiscordSocketClient _client;
        private readonly DatabaseService _databaseService;

        public RepacksTrackerFactory(DiscordSocketClient client, DatabaseService databaseService)
        {
            _client = client;
            _databaseService = databaseService;
        }

        public RepacksTracker CreateRepacksTracker()
        {
            return new RepacksTracker(_client, _databaseService);
        }
    }
}
