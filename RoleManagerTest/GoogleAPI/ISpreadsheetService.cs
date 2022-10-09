namespace RoleManagerTest.GoogleAPI
{
    public interface ISpreadsheetService
    {
        public Task<IList<IList<object>>> GetSheetData();

    }
}
