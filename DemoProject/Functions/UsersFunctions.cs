using System.Collections.Generic;
using System.Net;
using DemoProject.DataModel;
using DemoProject.Extensions;
using DemoProject.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DemoProject.Functions
{
    public class UsersFunctions
    {
        private readonly ILogger _logger;
        private readonly IUserCosmos _userCosmos;

        public UsersFunctions(ILoggerFactory loggerFactory, IUserCosmos userCosmos)
        {
            _logger = loggerFactory.CreateLogger<UsersFunctions>();
            _userCosmos = userCosmos;
        }

        [Function("UsersFunctions")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        /// <summary>
        /// CreateContainer for Only one use
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("CreateContainer")]
        public async Task<HttpResponseData> CreateContainer([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
        {
            _logger.LogInformation("CreateNewUsers Azure Function has been hit");
            try
            {
                var data = await _userCosmos.CreateContainer();
                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        /// <summary>
        /// AddNewUsersInfo Function
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("AddNewUsersInfo")]
        public async Task<HttpResponseData> AddNewUsersInfo([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("AddNewUsersInfo Azure Function has been hit");
            try
            {
                var userData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.AddNewUsersInfo(userData);

                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("GetAllUserData")]
        [Authorize]
        public async Task<HttpResponseData> GetAllUserData([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
        {
            _logger.LogInformation("AddNewUsersInfo Azure Function has been hit");
            try
            {
                var data = await _userCosmos.GetAllUserData();
                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }


        [Function("GetUserDataIdwise")]
        public async Task<HttpResponseData> GetUserDataIdwise([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("AddNewUsersInfo Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.GetUserDataIdwise(requestData.Id.ToString());

                return await request.Ok(data);

            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("UpdateUserData")]
        public async Task<HttpResponseData> UpdateUserData([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("AddNewUsersInfo Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.UpdateUserData(requestData);

                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("UpsertUserData")]
        public async Task<HttpResponseData> UpsertUserData([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("AddNewUsersInfo Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.UpsertUserData(requestData);

                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT Support");
            }
        }

        [Function("DeleteUserData")]
        public async Task<HttpResponseData> DeleteUserData([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("DeleteUserData Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.DeleteUserData(requestData.Id.ToString());

                return await request.Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an internal error: "+ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT ");
            }
        }

        [Function("UserLogIn")]
        public async Task<HttpResponseData> UserLogIn([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
        {
            _logger.LogInformation("UserLogIn Azure Function has been hit");
            try
            {
                var requestData = await request.ReadFromJsonAsync<Users>();
                var data = await _userCosmos.UserLogIn(requestData);
                if (data != null)
                {
                    return await request.Ok(data);
                }

                return await request.UnAuthorized("You do not have any credentials");
            }
            catch(Exception ex)
            {
                _logger.LogError("There was internal error : " + ex.Message.ToString());
                return await request.InternalServerError("There was an error, Please contact with IT");
            }
        }
    }
}

