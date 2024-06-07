using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace HydraBot.Commands
{
    public class ConfigurationCommands : BaseCommandModule
    {
        [Command("setnotificationchannel")]
        [RequirePermissions(DSharpPlus.Permissions.ManageGuild)]
        public async Task SetNotificationChannel(CommandContext ctx, DiscordChannel channel)
        {
            var config = ConfigManager.LoadConfig();
            config.NotificationChannelId = channel.Id;
            ConfigManager.SaveConfig(config);

            await ctx.RespondAsync($"Canal de notificação definido para {channel.Mention}");
        }
    }
}
