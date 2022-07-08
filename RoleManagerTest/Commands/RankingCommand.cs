using DatabaseClasses;
using DatabaseClasses.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using RoleManagerTest.Ranking;
using RoleManagerTest.Services.Interfaces;

namespace RoleManagerTest.Commands
{
    public class RankingCommand : BaseCommandModule
    {
        //private readonly UserClassContext _userClassContext;

        private readonly IStorageService<UserWoWChar> _storageService;

        private readonly HttpClient client = new HttpClient();

        public RankingCommand(IStorageService<UserWoWChar> storageService) //UserClassContext context)
        {
            _storageService = storageService;
            //this._userClassContext = context;
        }

        [Command("ScoreRanking")]
        [RequireGuild]
        [RequireBotPermissions(Permissions.SendMessages)]
        [Description("Returns the current M+ ranking of the discords members.")]
        public async Task GetScoreRanking(CommandContext ctx, [Description("The amount of characters to display. (not more than 20)")] int charAmount = 5)
        {
            if (charAmount > 20)
                return;

            var rankingList = await this.GetRankingList(ctx, true);
            if (rankingList.Count == 0)
                return;

            rankingList.Sort();

            var rankingAmount = rankingList.Count < charAmount ? rankingList.Count : charAmount;

            UserRanking[] ranking = new UserRanking[rankingAmount];
            for (int i = 0; i < ranking.Length; i++)
            {
                ranking[i] = rankingList[i];
            }

            var embed = this.BuildRankingDiscordEmbed(ctx, "The winner is: ", ranking);
            await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
        }

        private async Task<List<UserRanking>> GetRankingList(CommandContext ctx, bool compareByScore)
        {
            // var userClasses = this._userClassContext.UserClasses.ToList();
            var userClasses = this._storageService.Storage;

            List<UserRanking> rankings = new List<UserRanking>();
            foreach (var userClass in userClasses)
            {
                var charInfo = await this.GetCharacterInfo(userClass.CharName, userClass.ServerName, userClass.Region);
                if (charInfo == null)
                    continue;

                var member = await ctx.Guild.GetMemberAsync(userClass.DiscordID);
                if (charInfo.Mythic_plus_highest_level_runs.Length == 0)
                    continue;

                var scoresObj = charInfo.Mythic_plus_scores_by_season[0].Scores;
                if (scoresObj == null)
                    continue;

                if (scoresObj.Count() == 0)
                    continue;

                if (!scoresObj.TryGetValue("all", out string scoreString))
                    continue;

                scoreString = scoreString.Replace('.', ',');
                var score = double.Parse(scoreString);

                rankings.Add(new UserRanking(compareByScore)
                {
                    UserName = member.Username,
                    HighestRun = charInfo.Mythic_plus_highest_level_runs[0].Mythic_Level,
                    Dungeon = charInfo.Mythic_plus_highest_level_runs[0].DungeonName,
                    ClearTime = charInfo.Mythic_plus_highest_level_runs[0].Clear_Time_MS,
                    Score = score
                });
            }

            return rankings;
        }

        private DiscordEmbed BuildRankingDiscordEmbed(CommandContext ctx, string title, UserRanking[] ranking)
        {
            // build the embed
            DiscordEmbedBuilder blueprint = new DiscordEmbedBuilder();

            blueprint.Color = DiscordColor.Gold;
            blueprint.Title = title;

            // not allowed to have more than 25 fields!!!
            for (int i = ranking.Length - 1; i >= 0; i--)
            {
                var rank = ranking[i];
                blueprint.AddField($"{rank.UserName}:",
                    $"Score: {rank.Score} \n" + 
                    $"Highest Dungeon: {rank.Dungeon} {rank.HighestRun} \n"
                );
            }

            return blueprint.Build();
        }

        private async Task<RankingCharacter> GetCharacterInfo(string characterName, string serverName, string region)
        {
            // defines the query to search for a specified character
            string query;
            using (var parameters = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "region", $"{region}"},
                { "realm", $"{serverName}"},
                { "name", $"{characterName}"},
                { "fields", "mythic_plus_highest_level_runs,mythic_plus_scores_by_season:current"}
            }))
            {
                query = await parameters.ReadAsStringAsync();
            }

            // gets the specified character from raider.io api
            var url = @"https://raider.io/api/v1/characters/profile?" + query;

            var response = await client.GetAsync(url);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var characterInfo = JsonConvert.DeserializeObject<RankingCharacter>(content);

            return characterInfo;
        }
    }
}
