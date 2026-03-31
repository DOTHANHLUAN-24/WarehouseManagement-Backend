using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WarehouseManagement.BackendServer.Helpers
{
    public class ApiBadRequestResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; } = Array.Empty<string>();

        // Use this constructor when you want to provide a custom list of error messages
        public ApiBadRequestResponse(IEnumerable<string> errors) : base(400, "Bad request")
        {
            Errors = errors;
        }

        // Use this constructor when you want to provide a single custom error message
        public ApiBadRequestResponse(string message) : base(400, message) { }

        // Use this constructor when you want to extract error messages from an invalid ModelStateDictionary
        public ApiBadRequestResponse(ModelStateDictionary modelState) : base(400)
        {
            {
                if (modelState.IsValid)
                {
                    throw new ArgumentException("ModelState must be invalid", nameof(modelState));
                }

                Errors = modelState
                    .SelectMany(_ => _.Value!.Errors)
                    .Select(_ => _.ErrorMessage).ToArray();
            }
        }

        // Use this constructor when you want to extract error messages from an IdentityResult
        public ApiBadRequestResponse(IdentityResult identityResult) : base(400)
        {
            Errors = identityResult.Errors
                .Select(_ => _.Code + " - " + _.Description).ToArray();
        }


    }
}
