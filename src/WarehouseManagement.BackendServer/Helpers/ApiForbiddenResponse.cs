namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiForbiddenResponse : ApiResponse
    {
        // Default constructor sets status code to 403 and uses the default message for 403
        public ApiForbiddenResponse() : base(403) { }

        // Constructor that allows custom message while still setting status code to 403
        public ApiForbiddenResponse(string message) : base(403, message) { }
    }
}
