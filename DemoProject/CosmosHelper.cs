using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject
{
    public class CosmosHelper
    {
        private readonly string URI = "https://localhost:8081";
        private readonly string primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string dbName = "TestDB";
        private string containerName = "DataContainer";
        private string partitionPath = "/id";

        public CosmosHelper()
        {
            this.cosmosClient = new CosmosClient(URI, primaryKey);
        }

        public async Task<dynamic> CreateTestRecord()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerName, partitionPath);

            var insertData = new
            {
                id = Guid.NewGuid().ToString(),
                name = "TestInput"
            };
            var data = await this.container.CreateItemAsync(insertData, new PartitionKey(insertData.id));

            return data;
        }
    }
}
