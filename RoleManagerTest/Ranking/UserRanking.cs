namespace RoleManagerTest.Ranking
{
    public class UserRanking : IEquatable<UserRanking>, IComparable<UserRanking>
    {
        private bool compareByScore;

        public UserRanking(bool compareByScore)
        {
            this.compareByScore = compareByScore;
        }

        public string UserName { get; set; }

        public double Score { get; set; }

        public int HighestRun { get; set; }

        public string Dungeon { get; set; }

        public int ClearTime { get; set; }

        public int CompareTo(UserRanking? other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (compareByScore)
            {
                if (this.Score == other.Score)
                    return 0;

                if (this.Score > other.Score)
                    return 1;

                return -1;
            }

            if (this.HighestRun == other.HighestRun)
            {
                if (this.ClearTime == other.ClearTime)
                    return 0;

                if (this.ClearTime > other.ClearTime)
                    return 1;

                return -1;
            }  

            if (this.HighestRun > other.HighestRun)
                return 1;

            return -1;
        }

        public bool Equals(UserRanking? other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return this.UserName == other.UserName;
        }
    }
}
