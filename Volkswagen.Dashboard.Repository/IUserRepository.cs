using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volkswagen.Dashboard.Repository
{
    public interface IUserRepository
    {
        Task<bool> ExistWithEmail(string email);
        Task InsertUser(string email, string username, string password);
        Task<UserModel> GetUserByEmail(string email);
        Task<bool> IsEmailInWhitelist(string email);
    }
}
