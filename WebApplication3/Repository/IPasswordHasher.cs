using System.Security.Cryptography;
using System.Text;

namespace WebApplication3.Repository
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, string salt);
        string GenerateSalt();
    }
}
