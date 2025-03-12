using WebApplication3.Models;
using System.Threading.Tasks;


namespace WebApplication3.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByNameAsync(string name);
    }
}
