using DemoProject.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Repository.Interface
{
    public interface IUserCosmos
    {
        Task<object> CreateContainer();
        Task<Users> AddNewUsersInfo(Users userData);
        Task<List<Users>> GetAllUserData();
        Task<Users> GetUserDataIdwise(string v);
        Task<bool> UpdateUserData(Users requestData);
        Task<bool> UpsertUserData(Users requestData);
        Task<bool> DeleteUserData(string id);
    }
}
