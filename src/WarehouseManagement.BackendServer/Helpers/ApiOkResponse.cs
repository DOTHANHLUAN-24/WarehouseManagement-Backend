namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiOkResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiOkResponse(T data) : base(200)
        {
            Data = data;
        }

        public ApiOkResponse(T data, string message) : base(200, message)
        {
            Data = data;
        }

        public ApiOkResponse(string message) : base(200, message)
        {
            Data = default!;
        }
    }
}