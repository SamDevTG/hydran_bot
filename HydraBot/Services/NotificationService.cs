using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace HydraBot.Services
{
    public class NotificationService
    {
        private ulong _notificationChannelId;
        private readonly DiscordSocketClient _discordClient;

        public NotificationService(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public void SetNotificationChannel(ulong channelId)
        {
            _notificationChannelId = channelId;
        }

        public async Task SendNotificationAsync(string message)
        {
            if (_notificationChannelId == 0)
            {
                Console.WriteLine("Canal de notificação não definido. Ignorando notificação.");
                return;
            }

            var channel = _discordClient.GetChannel(_notificationChannelId) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }

        public async Task MonitorRepacks()
        {
            if (_notificationChannelId == 0)
            {
                Console.WriteLine("Canal de notificação não definido. Aguardando definição do canal...");
                return;
            }

            while (true)
            {
                var existingRepacks = _databaseService.GetExistingRepacks();
                var newRepacks = await GetNewRepacks(existingRepacks);

                if (newRepacks.Any())
                {
                    foreach (var repack in newRepacks)
                    {
                        await SendNotificationAsync($"Novo repack disponível: {repack.Title} - {repack.Magnet}");
                    }

                    _databaseService.SaveRepacks(newRepacks);
                }

                await Task.Delay(TimeSpan.FromHours(1));
            }
        }

        private async Task<IEnumerable<Repack>> GetNewRepacks(IEnumerable<Repack> existingRepacks)
        {
            // Lógica para obter novos repacks dos sites e comparar com os existentes
            // Por exemplo, pode-se usar a lógica atual do Hydra para isso

            return new List<Repack>(); // Retorna a lista de novos repacks encontrados
        }
    }
}
