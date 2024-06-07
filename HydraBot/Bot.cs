// Bot.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HydraBot.Models;
using HydraBot.Services;

namespace HydraBot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private readonly _1337xService _1337xService;
        private readonly GOGService _gogService;
        private readonly OnlineFixService _onlineFixService;
        private readonly XatabService _xatabService;
        private readonly DatabaseService _databaseService;
        private ulong _notificationChannelId;

        public Bot(
            DiscordSocketClient client,
            IConfiguration config,
            _1337xService _1337xService,
            GOGService gogService,
            OnlineFixService onlineFixService,
            XatabService xatabService,
            DatabaseService databaseService)
        {
            _client = client;
            _config = config;
            _1337xService = _1337xService;
            _gogService = gogService;
            _onlineFixService = onlineFixService;
            _xatabService = xatabService;
            _databaseService = databaseService;
        }

        public async Task RunAsync()
        {
            // Carregar o canal de notificação da configuração
            _notificationChannelId = _config.GetSection("NotificationChannelId").GetValue<ulong>();

            // Monitorar repacks de cada tracker
            await MonitorRepacks(_1337xService.GetNewRepacks);
            await MonitorRepacks(_gogService.GetNewRepacks);
            await MonitorRepacks(_onlineFixService.GetNewRepacks);
            await MonitorRepacks(_xatabService.GetNewRepacks);
        }

        private async Task MonitorRepacks(Func<IEnumerable<Repack>, Task<IEnumerable<Repack>>> getNewRepacks)
        {
            var existingRepacks = _databaseService.GetExistingRepacks();
            var newRepacks = await getNewRepacks(existingRepacks);

            if (newRepacks.Any() && _notificationChannelId != 0)
            {
                var channel = _client.GetChannel(_notificationChannelId) as IMessageChannel;
                foreach (var repack in newRepacks)
                {
                    // Notificar sobre o lançamento do repack
                    await channel.SendMessageAsync($"Novo repack disponível: {repack.Title} - {repack.Magnet}");
                }

                // Salvar os novos repacks no banco de dados
                _databaseService.SaveRepacks(newRepacks);
            }
        }
    }
}
