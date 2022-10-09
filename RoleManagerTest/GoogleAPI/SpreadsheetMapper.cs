namespace RoleManagerTest.GoogleAPI
{
    public static class SpreadsheetMapper
    {
        public static SpreadsheetCharacter[] MapFromRangeData(IList<IList<object>> values)
        {
            var items = new SpreadsheetCharacter[values.Count];
            for(int i = 0; i < values.Count; i++)
            {
                if (values[i] == null || values[i].Count < 6)
                {
                    items[i] = null;
                    continue;
                }

                SpreadsheetCharacter item;
                if (values[i].Count == 6)
                    item = new(values[i][0].ToString(), values[i][1].ToString(), values[i][2].ToString(), values[i][3].ToString(), values[i][4].ToString(), values[i][5].ToString(), string.Empty);
                else
                    item = new(values[i][0].ToString(), values[i][1].ToString(), values[i][2].ToString(), values[i][3].ToString(), values[i][4].ToString(), values[i][5].ToString(), values[i][6].ToString());

                items[i] = item;
            }
            return items;
        }

        public static IList<IList<object>> MapToRangeData(SpreadsheetCharacter item)
        {
            var objectList = new List<object>() { item.DiscordID, item.Region, item.ServerName, item.CharName, item.ClassName, item.SpeccName, item.Offspecc };
            var rangeData = new List<IList<object>> { objectList };
            return rangeData;
        }
    }
}
