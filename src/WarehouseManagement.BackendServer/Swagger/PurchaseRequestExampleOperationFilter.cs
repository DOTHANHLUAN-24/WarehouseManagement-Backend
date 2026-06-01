using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WarehouseManagement.BackendServer.Swagger
{
    public class PurchaseRequestExampleOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null)
                return;

            var path = context.ApiDescription.RelativePath ?? string.Empty;
            var method = context.ApiDescription.HttpMethod ?? string.Empty;

            // target PurchaseReceipt endpoints
            if (!path.StartsWith("api/PurchaseReceipt", System.StringComparison.OrdinalIgnoreCase) && !path.StartsWith("api/purchases", System.StringComparison.OrdinalIgnoreCase))
                return;
            if (!(method == "POST" || method == "PUT"))
                return;

            if (!operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
                return;

            // Request example: only include fields client should send. Remove system-generated fields.
            var example = new OpenApiObject
            {
                ["type"] = new OpenApiInteger(1), // 1: Nhập, 2: Xuất
                ["supplierId"] = new OpenApiInteger(0), // Required if type == 1
                ["customerId"] = new OpenApiInteger(0), // Required if type == 2
                ["warehouseId"] = new OpenApiInteger(0),
                ["supplierName"] = new OpenApiString("string"),
                ["customerName"] = new OpenApiString("string"),
                ["receiptDate"] = new OpenApiString("2026-05-19T15:30:00.000Z"),
                ["referenceCode"] = new OpenApiString("string"),
                ["note"] = new OpenApiString("string"),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["productId"] = new OpenApiInteger(0),
                        ["quantity"] = new OpenApiInteger(0),
                        ["unitCost"] = new OpenApiDouble(0.0)
                    }
                }
            };

            mediaType.Example = example;

            // Prepare response example object (what API returns)
            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(123),
                ["receiptCode"] = new OpenApiString("PO-20260519-001"),
                ["referenceCode"] = new OpenApiString("HD-12345"),
                ["note"] = new OpenApiString("Nhập hàng đợt 1 tháng 5"),
                ["supplierId"] = new OpenApiInteger(101),
                ["supplierName"] = new OpenApiString("Công ty TNHH ABC"),
                ["customerId"] = new OpenApiNull(),
                ["customerName"] = new OpenApiNull(),
                ["isExport"] = new OpenApiBoolean(false),
                ["type"] = new OpenApiInteger(1),
                ["purchaseDate"] = new OpenApiString("2026-05-19T15:30:00.000Z"),
                ["totalAmount"] = new OpenApiDouble(15000000),
                ["createdBy"] = new OpenApiString("802f1afd-89f3-45bd-b785-19346929326b"),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["productId"] = new OpenApiInteger(12),
                        ["productVariantId"] = new OpenApiInteger(12),
                        ["quantity"] = new OpenApiInteger(20),
                        ["unitCost"] = new OpenApiDouble(500000),
                        ["totalPrice"] = new OpenApiDouble(10000000)
                    }
                }
            };

            // Attach response examples for POST (201) and GET (200)
            // POST response (Created)
            if (method == "POST")
            {
                if (!operation.Responses.TryGetValue("201", out var postResp))
                {
                    postResp = new OpenApiResponse { Description = "Created" };
                    operation.Responses["201"] = postResp;
                }

                if (!postResp.Content.TryGetValue("application/json", out var postMedia))
                {
                    postMedia = new OpenApiMediaType();
                    postResp.Content["application/json"] = postMedia;
                }

                postMedia.Example = responseExample;
            }

            // GET responses: single or list
            if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    if (!operation.Responses.TryGetValue("200", out var getResp))
                    {
                        getResp = new OpenApiResponse { Description = "OK" };
                        operation.Responses["200"] = getResp;
                    }

                    if (!getResp.Content.TryGetValue("application/json", out var getMedia))
                    {
                        getMedia = new OpenApiMediaType();
                        getResp.Content["application/json"] = getMedia;
                    }

                    getMedia.Example = responseExample;
                }
                else
                {
                    // list response example
                    if (!operation.Responses.TryGetValue("200", out var listResp))
                    {
                        listResp = new OpenApiResponse { Description = "OK" };
                        operation.Responses["200"] = listResp;
                    }

                    if (!listResp.Content.TryGetValue("application/json", out var listMedia))
                    {
                        listMedia = new OpenApiMediaType();
                        listResp.Content["application/json"] = listMedia;
                    }

                    listMedia.Example = new OpenApiArray { responseExample };
                }
            }
        }
    }
}
