﻿namespace DatabaseClasses.Models
{
    public class UserWoWChar : Entity
    {
        public string DiscordID { get; set; }

        public string CharName { get; set; }

        public string ServerName { get; set; }

        public string Region { get; set; }
    }
}
