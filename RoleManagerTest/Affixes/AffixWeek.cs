namespace RoleManagerTest.Affixes
{
    public class AffixWeek
    {
        public AffixWeek(List<Affix> affixes)
        {
            this.Affixes = affixes;
        }

        public List<Affix> Affixes { get; private set; }
    }
}
