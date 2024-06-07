using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HydraBot.Models;
using HydraBot.Services;

namespace HydraBot.Trackers
{
    public class RepacksTracker
    {
        private readonly DiscordSocketClient _client;
        private readonly DatabaseService _databaseService;
        private ulong _notificationChannelId;

        public RepacksTracker(DiscordSocketClient client, DatabaseService databaseService)
        {
            _client = client;
            _databaseService = databaseService;
        }

        public async Task StartTracking(ulong notificationChannelId)
        {
            _notificationChannelId = notificationChannelId;
            await MonitorRepacks();
        }

        private async Task MonitorRepacks()
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
            // Lógica para obter novos repacks dos trackers de repack do Hydra
            return newRepacks;
        }
    }
}
