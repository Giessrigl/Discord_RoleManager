using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoleManagerTest
{
    public static class Statics
    {
        public static ulong classes_and_roles_ChannelID = 991125120993214535;
        public static string GetCommandDescription(Type commandType, string methodName)
        {
            if (commandType == null || string.IsNullOrWhiteSpace(methodName))
                return "";

            MethodBase method = commandType.GetMethod(methodName);

            DescriptionAttribute description =
                (DescriptionAttribute)method.GetCustomAttribute(typeof(DescriptionAttribute));

            CommandAttribute command =
                (CommandAttribute)method.GetCustomAttribute(typeof(CommandAttribute));

            if (description != null && command != null)
                return command.Name + ": " + description.Description;

            return "";
        }
    }
}
