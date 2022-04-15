using DemoProject.BodyModel;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string BadRequestErrorTypeUrl = "https://httpstatuses.com/400";
        private const string Title = "One or more validation errors occurred.";

        public static async Task<HttpResponseData> BadRequest(this HttpRequestData requestData, string errorMessage)
        {
            var response = requestData.CreateResponse(HttpStatusCode.BadRequest);

            var messageBody = new
            {
                Type = BadRequestErrorTypeUrl,
                Status = HttpStatusCode.BadRequest,
                Title = Title,
                Detail = errorMessage,
                Instance = requestData.Url.AbsoluteUri
            };

            await response.WriteAsJsonAsync(messageBody, HttpStatusCode.BadRequest);

            return response;
        }

        public static async Task<HttpResponseData> InternalServerError(this HttpRequestData requestData, string errorMessage)
        {
            var response = requestData.CreateResponse(HttpStatusCode.InternalServerError);

            var messageBody = new
            {
                Type = BadRequestErrorTypeUrl,
                Status = HttpStatusCode.BadRequest,
                Title = Title,
                Detail = errorMessage,
                Instance = requestData.Url.AbsoluteUri
            };
            
            await response.WriteAsJsonAsync(messageBody, HttpStatusCode.InternalServerError);

            return response;
        }



        public static async Task<HttpResponseData> Ok<TRest>(this HttpRequestData requestData, TRest result)
        {
            var response = requestData.CreateResponse(HttpStatusCode.OK);
            var responseBode = new ResponseModel();

            if (result == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                responseBode = new ResponseModel()
                {
                    Model = result
                };
                await response.WriteAsJsonAsync(responseBode, HttpStatusCode.NotFound);
                return response;
            }

            responseBode = new ResponseModel()
            {
                Model = result
            };
            await response.WriteAsJsonAsync(responseBode);
            return response;
        }

        public static async Task<HttpResponseData> UnAuthorized(this HttpRequestData requestData, string errorMessage)
        {
            var response = requestData.CreateResponse(HttpStatusCode.Unauthorized);

            var messageBody = new
            {
                Type = BadRequestErrorTypeUrl,
                Status = HttpStatusCode.Unauthorized,
                Title = Title,
                Detail = errorMessage,
                Instance = requestData.Url.AbsoluteUri
            };

            await response.WriteAsJsonAsync(messageBody, HttpStatusCode.Unauthorized);

            return response;
        }
    }
}
