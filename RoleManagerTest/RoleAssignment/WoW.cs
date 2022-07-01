using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest
{
    public static class WoW
    {
        public static List<WoWClass> Classes = new List<WoWClass>()
            {
                new WoWClass(
                    "DeathKnight",
                    991105618679185438,
                    992063098435416064,
                    new List<string>() // wow role name
                    {
                        "Tank",
                        "Melee"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more corruption upon this world by selecting the Death Knight...."
                    }),
                new WoWClass(
                    "DemonHunter",
                    991105736631386154,
                    992063056467202058,
                    new List<string>() // wow role name
                    {
                        "Tank",
                        "Melee"
                    },new List<string>() // class change messages
                    {
                        "chose to kill themself by selecting the Demon Hunter...."
                    }),
                new WoWClass(
                    "Druid",
                    991105834866200689,
                    992063110519210105,
                    new List<string>() // wow role name
                    {
                        "Tank",
                        "Heal",
                        "Melee",
                        "Range"
                    }, 
                    new List<string>() // class change messages
                    {
                        "chose to be indecisive by selecting the Druid...."
                    }),
                new WoWClass(
                    "Evoker",
                    991105930366296094,
                    default,
                    new List<string>() // wow role name
                    {
                        "Range",
                        "Heal"
                    }, new List<string>() // class change messages
                    {
                        "chose to fly over this world by selecting the Evoker...."
                    }),
                new WoWClass(
                    "Hunter",
                    991106047416737893,
                    992063120677810227,
                    new List<string>() // wow role name
                    {
                        "Range",
                        "Melee"
                    }, new List<string>() // class change messages
                    {
                        "chose to let more arrows rain down upon this world by selecting the Hunter...."
                    }),
                new WoWClass(
                    "Mage",
                    991106148327505931,
                    992063130458927184,
                    new List<string>() // wow role name
                    {
                        "Range"
                    }, new List<string>() // class change messages
                    {
                        "chose to study the mysteries of this world by selecting the Mage...."
                    }),
                new WoWClass(
                    "Monk",
                    991106242984558723,
                    992063140051300362,
                    new List<string>() // wow role name
                    {
                        "Tank",
                        "Melee",
                        "Heal"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more peace and fists into this world by selecting the Monk...."
                    }),
                new WoWClass(
                    "Paladin",
                    991106344625115186,
                    992063150486716587,
                    new List<string>() // wow role name
                    {
                        "Tank",
                        "Melee",
                        "Heal"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more holy light into this world by selecting the Paladin...."
                    }),
                new WoWClass(
                    "Priest",
                    991106410278572152,
                    992063161857474681,
                    new List<string>() // wow role name
                    {
                        "Range",
                        "Heal"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more holy water into this world by selecting the Priest...."
                    }),
                new WoWClass(
                    "Rogue",
                    991106498543501362,
                    992063173597339678,
                    new List<string>() // wow role name
                    {
                        "Melee"
                    }, new List<string>() // class change messages
                    {
                        "chose to steal from the poor and give it to themself by selecting the Rogue...."
                    }),
                new WoWClass(
                    "Shaman",
                    991106586061840444,
                    992063185731461140,
                    new List<string>() // wow role name
                    {
                        "Melee",
                        "Range",
                        "Heal"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring \"the elements\" to a gun fight by selecting the Shaman...."
                    }),
                new WoWClass(
                    "Warlock",
                    991106687471714414,
                    992063197068664942,
                    new List<string>() // wow role name
                    {
                        "Range"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more demons into this world by selecting the Warlock...."
                    }),
                new WoWClass(
                    "Warrior",
                    991106818661158924,
                    992063207554429018,
                    new List<string>() // wow role name
                    {
                         "Tank",
                         "Melee"
                    }, new List<string>() // class change messages
                    {
                        "chose to bring more spin to win into this world by selecting the Warrior...."
                    })
            };

        public static List<WoWRole> Roles = new List<WoWRole>()
        {
            new WoWRole("Tank", 991106887510675568, 992066661509247047),
            new WoWRole("Heal", 991106988400443453, 992066646829174794),
            new WoWRole("Melee", 991107301786255471, 992066621269094410),
            new WoWRole("Range", 991107142536937602, default)
        };
    }
}
