namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiUnauthorizedResponse : ApiResponse
    {
        public ApiUnauthorizedResponse(string message) : base(401, message) { }

        public ApiUnauthorizedResponse() : base(401) { }
    }
}
