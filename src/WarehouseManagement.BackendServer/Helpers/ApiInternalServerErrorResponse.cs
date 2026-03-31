namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiInternalServerErrorResponse : ApiResponse
    {
        public string? Details { get; set; }

        public ApiInternalServerErrorResponse(string message) : base(500, message) { }

        public ApiInternalServerErrorResponse(string message, string? details) : base(500, message)
        {
            Details = details;
        }
    }
}
