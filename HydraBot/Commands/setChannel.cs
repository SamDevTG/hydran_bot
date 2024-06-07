using HydraBot.Services;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HydraBot.Commands
{
    public class ConfigurationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly NotificationService _notificationService;

        public ConfigurationCommands(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Command("setnotificationchannel")]
        [RequireUserPermission(DSharpPlus.Permissions.ManageGuild)]
        public async Task SetNotificationChannel(SocketTextChannel channel)
        {
            _notificationService.SetNotificationChannel(channel.Id);
            await ReplyAsync($"Canal de notificação definido para {channel.Mention}");
        }
    }
}
