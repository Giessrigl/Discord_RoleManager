using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest
{
    public class WoWRole
    {
        public WoWRole(string name, ulong id, ulong emojiID)
        {
            Name = name;
            Id = id;
            EmojiID = emojiID;
        }

        public string Name { get; set; }
        public ulong Id { get; set; }

        public ulong EmojiID { get; set; }
    }
}
