using Newtonsoft.Json;

namespace RoleManagerTest.Ranking
{
    public class MythicPlusRun
    {
        [JsonProperty("dungeon")]
        public string DungeonName { get; set; }

        [JsonProperty("mythic_level")]
        public int Mythic_Level { get; set; }

        [JsonProperty("clear_time_ms")]
        public int Clear_Time_MS { get; set; }

    }
}
