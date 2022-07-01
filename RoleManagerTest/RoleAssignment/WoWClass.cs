using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest
{
    public class WoWClass
    {
        public WoWClass(string className, ulong id, ulong emojiID, List<string> roles)
        {
            this.Name = className;
            this.ClassID = id;
            this.EmojiID = emojiID;
            this.WoWRolesNames = roles;
            this.WelcomeMessages = new List<string>();
            
        }

        public WoWClass(string className, ulong id, ulong emojiID, List<string> roles, List<string> welcomeMessages)
        {
            this.Name = className;
            this.ClassID = id;
            this.EmojiID = emojiID;
            this.WoWRolesNames = roles;
            this.WelcomeMessages = welcomeMessages;
        }

        public string Name { get; private set; }
        public ulong ClassID { get; private set; }

        public ulong EmojiID { get; private set; }

        public List<string> WelcomeMessages { get; private set; }

        public List<string> WoWRolesNames { get; private set; }
    }
}
