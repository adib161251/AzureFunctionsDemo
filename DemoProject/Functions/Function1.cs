using System.Collections.Generic;
using System.Net;
using DemoProject.DataModel;
using DemoProject.Extensions;
using DemoProject.Repository.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DemoProject
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IFamilyCosmos _familyCosmos;


        public Function1(ILoggerFactory loggerFactory, IFamilyCosmos familyCosmos)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _familyCosmos = familyCosmos;
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }


        [Function("DemoFunction")]
        public async Task<HttpResponseData> DemoFunction([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
        {
            _logger.LogInformation("C# HttpTrigger Demo Function is being deployed");

            var response = new
            {
                MessageBody = "Demo Request Has been sent"
            };

            return await request.Ok(response);
        }

        [Function("TestCosmosRun")]
        public async Task<HttpResponseData> TestCosmosRun([HttpTrigger(AuthorizationLevel.Function, "get", Route ="TestCosmosRun1")] HttpRequestData request)
        {
            _logger.LogInformation("Test Cosmos has ben run");

            var cosmosDB = new CosmosHelper();
            var data = await cosmosDB.CreateTestRecord();

            var response = new
            {
                MessageBody = "Demo Request Has been sent"
            };

            return await request.Ok(response);

        }


        [Function("AddFamilyDataAsync")]
        public async Task<HttpResponseData> AddFamilyDataAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("AddFamilyDataAsync was hit");

            try
            {
                var requestData = await request.ReadFromJsonAsync<Family>();
                var data = await _familyCosmos.AddFamilyDataAsync(requestData);
                return await request.Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message+" Exception occured");
                return await request.InternalServerError(ex.Message.ToString());
            }
        }


        [Function("CreateNewDatabase")]
        public async Task<HttpResponseData> CreateNewDatabase([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("CreateNewDatabase Azure Function has been hit");
            try
            {
                var data = await _familyCosmos.CreateNewDatabase();
                return await request.Ok((object)data);
            }
            catch(Exception ex)
            {
                _logger.LogError("There was an error, Please contact IT Support : "+ ex.Message);
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("GetAllFamilyInfo")]
        public async Task<HttpResponseData> GetAllFamilyInfo([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
        {
            _logger.LogInformation("GetAllFamilyInfo Azure Function has been hit");
            try
            {
                var data = await _familyCosmos.GetAllFamilyInfo();
                //var data = await _familyCosmos.GetAllFamilyInfosAsyncV2();
                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error, " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        
        [Function("GetFamilyInfobyId")]
        public async Task<HttpResponseData> GetFamilyInfobyId([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("GetFamilyInfobyId Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Family>();
                //var data = await _familyCosmos.GetFamilyInfobyId(requestData.Id.ToString());
                //var data = await _familyCosmos.GetFamilyInfobyIdV2(requestData.Id.ToString());
                var data = await _familyCosmos.GetFamilyInfobyIdV3(requestData.Id.ToString());

                return await request.Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError("There was an error, " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("DeleteFamilyInfobyId")]
        public async Task<HttpResponseData> DeleteFamilyInfobyId([HttpTrigger(AuthorizationLevel.Function,"post")] HttpRequestData request)
        {
            _logger.LogInformation("DeleteFamilyInfobyId Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Family>();
                var data = await _familyCosmos.DeleteFamilyById(requestData.Id.ToString());

                return await request.Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError("There was an internal error : "+ ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }

        }

        [Function("UpdateFamilyInfo")]
        public async Task<HttpResponseData> UpdateFamilyInfo([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("DeleteFamilyInfobyId Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Family>();
                var data = await _familyCosmos.UpdateFamilyData(requestData);

                return await request.Ok(data);

            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error : " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }

        }
    }
}
