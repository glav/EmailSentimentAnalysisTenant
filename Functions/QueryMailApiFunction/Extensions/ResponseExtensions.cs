using QueryMailApiFunction.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace QueryMailApiFunction.Extensions
{
    public static class ResponseExtensions
    {
        public static HttpResponseMessage ToHttpResponseMessage<T>(this ApiResponse<T> apiResponse, HttpRequestMessage req) where T : class
        {
            if (apiResponse.HasError)
            {
                return req.CreateErrorResponse(apiResponse.ErrorCode, apiResponse.ErrorMessage);
            }
            return req.CreateResponse(apiResponse.ResponseData);

        }
    }
}
