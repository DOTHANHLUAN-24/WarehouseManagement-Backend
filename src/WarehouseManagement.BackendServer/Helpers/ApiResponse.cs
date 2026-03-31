using Newtonsoft.Json;

namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiResponse
    {
        public int StatusCode { set; get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Message { set; get; }

        public ApiResponse(int statusCode, string? message = null!)
        {
            {
                StatusCode = statusCode;
                Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            }
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                // 2xx Success
                case 200:
                    return "Request successful";
                case 201:
                    return "Resource created successfully";
                case 204:
                    return "No content";

                // 4xx Client errors
                case 400:
                    return "Bad request";
                case 401:
                    return "Unauthorized";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Resource not found";
                case 405:
                    return "Method not allowed";
                case 408:
                    return "Request timeout";
                case 409:
                    return "Conflict occurred";
                case 415:
                    return "Unsupported media type";
                case 422:
                    return "Unprocessable entity";

                // 5xx Server errors
                case 500:
                    return "An unhandled error occurred";
                case 501:
                    return "Not implemented";
                case 502:
                    return "Bad gateway";
                case 503:
                    return "Service unavailable";
                case 504:
                    return "Gateway timeout";

                default:
                    return "Unknown status";
            }
        }
    }
}
