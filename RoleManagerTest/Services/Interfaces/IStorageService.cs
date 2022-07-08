namespace RoleManagerTest.Services.Interfaces
{
    public interface IStorageService<T> where T : class
    {
        public List<T> Storage { get; }
    }
}
