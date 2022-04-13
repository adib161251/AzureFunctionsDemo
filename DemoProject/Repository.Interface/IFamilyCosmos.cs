using DemoProject.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Repository.Interface
{
    public interface IFamilyCosmos
    {
        Task<Family> AddFamilyDataAsync(Family data);
        Task<object> CreateNewDatabase();
        Task<List<Family>> GetAllFamilyInfo();

        Task<List<Family>> GetAllFamilyInfosAsyncV2();
        Task<Family> GetFamilyInfobyId(string id);
        Task<Family> GetFamilyInfobyIdV2(string id);
        Task<Family> GetFamilyInfobyIdV3(string id);
        Task<bool> DeleteFamilyById(string id);
        Task<bool> UpdateFamilyData(Family family);
        Task<bool> UpsertFamilyData(Family family);
    }
}
