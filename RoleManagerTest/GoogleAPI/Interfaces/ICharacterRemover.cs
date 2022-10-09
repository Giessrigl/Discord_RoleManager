using DSharpPlus;
using DSharpPlus.EventArgs;

namespace RoleManagerTest.GoogleAPI.Interfaces
{
    public interface ICharacterRemover
    {
        public Task DeleteChars(DiscordClient client, GuildMemberRemoveEventArgs args);

        public Task DeleteChar(DiscordClient client, string charName);
    }
}
