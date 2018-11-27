using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QueryMailApiFunction.Domain
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse(T apiResponseData)
        {
            ResponseData = apiResponseData;
        }

        public ApiResponse(HttpStatusCode apiErrorCode, string apiErrorMessage)
        {
            ErrorCode = apiErrorCode;
            ErrorMessage = apiErrorMessage;
        }
        public T ResponseData { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode ErrorCode {get; set; }
        public bool HasError => (!string.IsNullOrWhiteSpace(ErrorMessage) || (int)ErrorCode >= 400);
    }
}
