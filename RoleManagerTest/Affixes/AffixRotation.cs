namespace RoleManagerTest.Affixes
{
    public class AffixRotation
    {
        public int currentWeekIndex { get; private set; }

        public List<AffixWeek> Affixes { get; private set; }

        public AffixRotation()
        {
            this.currentWeekIndex = 9;
            this.Affixes = new List<AffixWeek>() 
            { 
                new AffixWeek(new List<Affix>() 
                    { 
                        new Affix("Tyrannical"),
                        new Affix("Bolstering"),
                        new Affix("Explosive"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    { 
                        new Affix("Fortified"),
                        new Affix("Bursting"),
                        new Affix("Storming"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Tyrannical"),
                        new Affix("Raging"),
                        new Affix("Volcanic"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Fortified"),
                        new Affix("Inspiring"),
                        new Affix("Grievous"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Tyrannical"),
                        new Affix("Spiteful"),
                        new Affix("Necrotic"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Fortified"),
                        new Affix("Bolstering"),
                        new Affix("Quaking"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Tyrannical"),
                        new Affix("Sanguine"),
                        new Affix("Storming"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Fortified"),
                        new Affix("Raging"),
                        new Affix("Explosive"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Tyrannical"),
                        new Affix("Bursting"),
                        new Affix("Volcanic"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Fortified"),
                        new Affix("Spiteful"),
                        new Affix("Necrotic"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Tyrannical"),
                        new Affix("Inspiring"),
                        new Affix("Quaking"),
                    }
                ),
                new AffixWeek(new List<Affix>()
                    {
                        new Affix("Fortified"),
                        new Affix("Sanguine"),
                        new Affix("Grievous"),
                    }
                ),
            };
        }

        public AffixWeek GetAffixWeek()
        {
            currentWeekIndex++;

            if (currentWeekIndex >= Affixes.Count)
                currentWeekIndex = 0;

            return Affixes[currentWeekIndex];
        }
    }
}
