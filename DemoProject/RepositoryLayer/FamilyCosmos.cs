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
    public class FamilyCosmos : IFamilyCosmos
    {
        private readonly string URI = "https://localhost:8081";
        private readonly string primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string dbName = "FamilyTree";
        private string containerName = "FamilyMember";
        private string partitionPath = "/id";

        public FamilyCosmos()
        {
            this.cosmosClient = new CosmosClient(URI, primaryKey);
            this.container = cosmosClient.GetContainer(dbName, containerName);
        }

        public async Task<object> CreateNewDatabase()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerName, partitionPath);

            return container;
        }

        public async Task<Family> AddFamilyDataAsync(Family data)
        {
            data.InsertedOn = DateTime.Now;
            data.Id = Guid.NewGuid();

            var responseData = await container.CreateItemAsync<Family>(data);
            return responseData;
        }

        public async Task<List<Family>> GetAllFamilyInfo()
        {
            var queryString = "Select * from c";

            var query = container.GetItemQueryIterator<Family>(queryString);

            var list = new List<Family>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                list.AddRange(response.ToList());
            }

            return list;
        }

        public async Task<List<Family>> GetAllFamilyInfosAsyncV2()
        {
            var linq = container.GetItemLinqQueryable<Family>().ToFeedIterator();
            //var result = await linq.ReadNextAsync();

            var list = new List<Family>();
            while (linq.HasMoreResults)
            {
                var response = await linq.ReadNextAsync();
                list.AddRange(response.ToList());
            }

            return list;
        }


        //Doesn't work
        public async Task<Family> GetFamilyInfobyId(string id)
        {
            var query = "select * from c from where c.id = @id";

            var queryDefinition = new QueryDefinition(query).WithParameter("@id", id);

            FeedIterator streamResultSet = container.GetItemQueryStreamIterator(queryDefinition,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(id)
                }
                );

            var list = new List<Family>();
            while (streamResultSet.HasMoreResults)
            {
                using (ResponseMessage responseMessage = await streamResultSet.ReadNextAsync())
                {

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        dynamic streamResponse = responseMessage.Content;
                        List<Family> familyResult = streamResponse.Documents.ToObject<List<Family>>();
                        list.AddRange(familyResult);
                    }
                }
            }

            return list.FirstOrDefault();
        }
        //Works
        public async Task<Family> GetFamilyInfobyIdV2(string id)
        {

            //ItemResponse<Item> response = await this._container.ReadItemAsync<Item>(id, new PartitionKey(id));
            ItemResponse<Family> response = await container.ReadItemAsync<Family>(id, new PartitionKey(id));

            return response.Resource;
        }


        //Alternate solution for GetFamilyInfobyIdV2
        public async Task<Family> GetFamilyInfobyIdV3(string id)
        {
            var query = container.GetItemLinqQueryable<Family>();

            var iterator = query.Where(x=> x.Id.ToString() == id).ToFeedIterator();
            var list = new List<Family>();
            while(iterator.HasMoreResults)
            {
                var res = await iterator.ReadNextAsync();

                list.AddRange(res.ToList());
            }

            return list.FirstOrDefault();
        }

        //Delete UserData from storage
        public async Task<bool> DeleteFamilyById(string id)
        {
            //var isDataExist = await container.ReadItemAsync<Family>(id, new PartitionKey(id));
            var query = container.GetItemLinqQueryable<Family>();
            //var isDataExist = query.Where(x => x.Id.ToString() == id).ToFeedIterator();
            var isDataExist = await query.Where(x => x.Id.ToString() == id).CountAsync();

            if (isDataExist > 0)
            {
                var delete = await container.DeleteItemAsync<Family>(id, new PartitionKey(id));

                return true;
            }

            return false;
        }


        //Update UserData from storage
        public async Task<bool> UpdateFamilyData(Family family)
        {
            var query = container.GetItemLinqQueryable<Family>();
            var data =  query.Where(x => x.Id.ToString().Equals(family.Id.ToString())).ToFeedIterator();

            var familyData = (await data.ReadNextAsync()).ToList();
            
            if(familyData.Count > 0)
            {
                family.UpdatedOn = DateTime.Now.ToString();
                var updated = await container.ReplaceItemAsync<Family>(family,family.Id.ToString(), new PartitionKey(family.Id.ToString()));
                return true;
            }

            return false;
        }


        //Upsert UserData from Storage
        public async Task<bool> UpsertFamilyData(Family family)
        {
            await container.UpsertItemAsync<Family>(family, new PartitionKey(family.Id.ToString()));
            return true;
        }

    }
}

