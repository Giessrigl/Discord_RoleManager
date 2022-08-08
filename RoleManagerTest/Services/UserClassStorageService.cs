using RoleManagerTest.Services.Interfaces;

namespace RoleManagerTest.Services
{
    public class UserClassStorageService<T> : IStorageService<T> where T : class
    {
        private readonly List<T> userWoWChars;

        public List<T> Storage => this.userWoWChars;

        public UserClassStorageService()
        {
            this.userWoWChars = new List<T>();
        }
    }
}
