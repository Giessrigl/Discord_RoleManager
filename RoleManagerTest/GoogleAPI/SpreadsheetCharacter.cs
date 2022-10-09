namespace RoleManagerTest.GoogleAPI
{
    public class SpreadsheetCharacter : IEquatable<SpreadsheetCharacter>
    {
        public string DiscordID;
        public string Region;
        public string CharName;
        public string ClassName;
        public string SpeccName;
        public string ServerName;
        public string Offspecc;

        public SpreadsheetCharacter(string discordID, string region, string serverName, string charName, string className, string speccName, string offspecc = "")
        {            
            this.DiscordID = discordID;
            this.Region = region;
            this.ServerName = serverName;
            this.CharName = charName;
            this.ClassName = className;
            this.SpeccName = speccName;
            this.Offspecc = offspecc;
        }

        public bool Equals(SpreadsheetCharacter? other)
        {
            if (other == null) 
                return false;

            return this.DiscordID == other.DiscordID;
        }
    }
}
