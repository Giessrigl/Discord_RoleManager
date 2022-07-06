using Newtonsoft.Json;

namespace RoleManagerTest.Ranking
{
    public class MythicPlusScore
    {
        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("scores")]
        public Dictionary<string, string> Scores { get; set; }

    }
}
