using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest
{
    public static class WoW
    {
        public static readonly List<WoWClass> Classes = new()
            {
                new WoWClass(
                    "Death Knight",
                    991105618679185438,
                    992063098435416064,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Blood", "Tank" },
                        {"Frost" ,"Melee" },
                        {"Unholy" ,"Melee" }
                    }),
                new WoWClass(
                    "Demon Hunter",
                    991105736631386154,
                    992063056467202058,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Havoc", "Tank" },
                        {"Vengeance" ,"Melee" }
                    }),
                new WoWClass(
                    "Druid",
                    991105834866200689,
                    992063110519210105,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Guardian", "Tank" },
                        {"Feral" ,"Melee" },
                        {"Balance", "Range" },
                        {"Restoration" ,"Heal" }
                    }),
                new WoWClass(
                    "Evoker",
                    991105930366296094,
                    994344346474270740,
                    new Dictionary<string, string>() // wow role name
                    {
                       {"Devastation", "Range" },
                       {"Preservation" ,"Heal" }
                    }),
                new WoWClass(
                    "Hunter",
                    991106047416737893,
                    992063120677810227,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Survival", "Melee" },
                        {"Beast Mastery" ,"Range" },
                        {"Marksmanship", "Range" }
                    }),
                new WoWClass(
                    "Mage",
                    991106148327505931,
                    992063130458927184,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Fire", "Range" },
                        {"Frost" ,"Range" },
                        {"Arcane", "Range" }
                    }),
                new WoWClass(
                    "Monk",
                    991106242984558723,
                    992063140051300362,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Brewmaster", "Tank" },
                        {"Mistweaver" ,"Heal" },
                        {"Windwalker", "Melee" }
                    }),
                new WoWClass(
                    "Paladin",
                    991106344625115186,
                    992063150486716587,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Holy", "Heal" },
                        {"Protection" ,"Tank" },
                        {"Retribution", "Melee" }
                    }),
                new WoWClass(
                    "Priest",
                    991106410278572152,
                    992063161857474681,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Discipline", "Heal" },
                        {"Holy" ,"Heal" },
                        {"Shadow", "Range" }
                    }),
                new WoWClass(
                    "Rogue",
                    991106498543501362,
                    992063173597339678,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Assassination", "Melee" },
                        {"Outlaw" ,"Melee" },
                        {"Subtlety", "Melee" }
                    }),
                new WoWClass(
                    "Shaman",
                    991106586061840444,
                    992063185731461140,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Enhancement", "Melee" },
                        {"Elemental" ,"Range" },
                        {"Restoration", "Heal" }
                    }),
                new WoWClass(
                    "Warlock",
                    991106687471714414,
                    992063197068664942,
                    new Dictionary<string, string>() // wow role name
                    {
                        {"Affliction", "Range" },
                        {"Demonology" ,"Range" },
                        {"Destruction", "Range" }
                    }),
                new WoWClass(
                    "Warrior",
                    991106818661158924,
                    992063207554429018,
                    new Dictionary<string, string>() // wow role name
                    {
                         {"Protection", "Tank" },
                        {"Arms" ,"Melee" },
                        {"Fury" ,"Melee" }
                    })
            };

        public static readonly List<WoWRole> Roles = new()
        {
            new WoWRole("Tank", 991106887510675568, 992066661509247047),
            new WoWRole("Heal", 991106988400443453, 992066646829174794),
            new WoWRole("Melee", 991107301786255471, 992066621269094410),
            new WoWRole("Range", 991107142536937602, default)
        };

        public static readonly List<WoWRole> OffspecRoles = new()
        {
            new WoWRole("Tank Offspec", 1018618887627083838, default),
            new WoWRole("Heal Offspec", 1018619027758792775, default),
            new WoWRole("Melee Offspec", 1018619291991560252, default),
            new WoWRole("Range Offspec", 1018619185858879488, default),
        };

        public static readonly Dictionary<string, DiscordColor> ClassColors = new()
        {
            { "Death Knight", new DiscordColor("#C41E3A")},
            { "Demon Hunter", new DiscordColor("#A330C9")},
            { "Druid", new DiscordColor("#FF7C0A")},
            { "Evoker", new DiscordColor("#33937F")},
            { "Hunter", new DiscordColor("#AAD372")},
            { "Mage", new DiscordColor("#3FC7EB")},
            { "Monk", new DiscordColor("#00FF98")},
            { "Paladin", new DiscordColor("#F48CBA")},
            { "Priest", new DiscordColor("#FFFFFF")},
            { "Rogue", new DiscordColor("#FFF468")},
            { "Shaman", new DiscordColor("#0070DD")},
            { "Warlock", new DiscordColor("#8788EE")},
            { "Warrior", new DiscordColor("#C69B6D")}
        };

        // Verifies if the enterd Offspecc is valid for the class.
        public static bool IsSpeccValid(string className, string specc)
        {
            var wowClass = Classes.FirstOrDefault(cl => cl.Name == className);
            if (wowClass == null)
                return false;

            if (wowClass.WoWRolesNames.ContainsKey(specc))
                return true;

            return false;
        }
    }
}
