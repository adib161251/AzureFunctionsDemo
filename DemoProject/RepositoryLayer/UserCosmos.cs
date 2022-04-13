using DemoProject.DataModel;
using DemoProject.Repository.Interface;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.RepositoryLayer
{
    public class UserCosmos : IUserCosmos
    {
        private readonly string URI = "https://localhost:8081";
        private readonly string primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string dbName = "FamilyTree";
        private string containerName = "UserInfo";
        private string partitionPath = "/id";


        public UserCosmos()
        {
            this.cosmosClient = new CosmosClient(URI,primaryKey);
            this.container = this.cosmosClient.GetContainer(dbName, containerName);
        }

        public async Task<object> CreateContainer()
        {
            var db = this.cosmosClient.GetDatabase(dbName);
            var container = await db.CreateContainerIfNotExistsAsync(containerName, partitionPath);

            return container;
        }

        public async Task<Users> AddNewUsersInfo(Users usersData)
        {
            usersData.Id = Guid.NewGuid();
            usersData.CreatedOn = DateTime.Now.ToString();

            var data = await container.CreateItemAsync<Users>(usersData);
            return data;
        }

        public async Task<List<Users>> GetAllUserData()
        {
            var query =  container.GetItemLinqQueryable<Users>();

            var iterator = query.ToFeedIterator();
            var list = new List<Users>();
            
            while(iterator.HasMoreResults)
            {
                var model = await iterator.ReadNextAsync();
                list.AddRange(model.ToList());
            }

            return list;
        }

        public async Task<Users> GetUserDataIdwise(string id)
        {
            var query = container.GetItemLinqQueryable<Users>();
            var iterator = query.Where(x=> x.Id.ToString().Equals(id)).ToFeedIterator();

            var list = new List<Users>();
            while(iterator.HasMoreResults)
            {
                var model = await iterator.ReadNextAsync();
                list.AddRange(model.ToList());
            }

            return list.FirstOrDefault();
        }

        public async Task<bool> UpdateUserData(Users requestData)
        {
            var query = container.GetItemLinqQueryable<Users>();

            var iterator = query.Where(x => x.Id.ToString().Equals(requestData.Id.ToString())).ToFeedIterator();
            var hasData = (await iterator.ReadNextAsync()).ToList();

            if(hasData.Count > 0)
            {
                requestData.UpdatedOn = DateTime.Now.ToString();
                var update = await container.ReplaceItemAsync<Users>(requestData, requestData.Id.ToString(), new PartitionKey(requestData.Id.ToString()));
                return true;
            }

            return false;
        }

        public async Task<bool> UpsertUserData(Users requestData)
        {
            await container.UpsertItemAsync<Users>(requestData, new PartitionKey(requestData.Id.ToString()));
            return true;
        }

        public async Task<bool> DeleteUserData(string id)
        {
            var query = container.GetItemLinqQueryable<Users>();
            var iterator = await query.Where(x => x.Id.ToString().Equals(id)).CountAsync();

            if(iterator > 0)
            {
                var delete = await container.DeleteItemAsync<Users>(id, new PartitionKey(id));
                return true;
            }
            return false;
        }
    }
}
