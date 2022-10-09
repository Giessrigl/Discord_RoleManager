namespace RoleManagerTest
{
    public class WoWClass : IEquatable<WoWClass>
    {
        public WoWClass(string className, ulong id, ulong emojiID, Dictionary<string, string> roles)
        {
            this.Name = className;
            this.ClassID = id;
            this.EmojiID = emojiID;
            this.WoWRolesNames = roles;
        }

        public string Name { get; private set; }
        public ulong ClassID { get; private set; }

        public ulong EmojiID { get; private set; }

        public Dictionary<string, string> WoWRolesNames { get; private set; }

        public bool Equals(WoWClass? other)
        {
            if (other == null) return false;

            return this.ClassID.Equals(other.ClassID);
        }
    }
}
