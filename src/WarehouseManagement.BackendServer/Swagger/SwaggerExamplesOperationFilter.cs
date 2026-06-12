using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WarehouseManagement.BackendServer.Swagger
{
    public class SwaggerExamplesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var path = context.ApiDescription.RelativePath ?? string.Empty;
            var method = context.ApiDescription.HttpMethod ?? string.Empty;

            var pathParts = path.Split('/');
            if (pathParts.Length < 2 || !pathParts[0].Equals("api", StringComparison.OrdinalIgnoreCase))
                return;

            var controller = pathParts[1];

            // 1. Categories
            if (controller.Equals("Categories", StringComparison.OrdinalIgnoreCase))
            {
                ApplyCategoriesExamples(operation, method, path);
            }
            // 2. Customers
            else if (controller.Equals("Customers", StringComparison.OrdinalIgnoreCase))
            {
                ApplyCustomersExamples(operation, method, path);
            }
            // 3. Products
            else if (controller.Equals("Products", StringComparison.OrdinalIgnoreCase))
            {
                ApplyProductsExamples(operation, method, path);
            }
            // 4. Suppliers
            else if (controller.Equals("Suppliers", StringComparison.OrdinalIgnoreCase))
            {
                ApplySuppliersExamples(operation, method, path);
            }
            // 5. Warehouses
            else if (controller.Equals("Warehouses", StringComparison.OrdinalIgnoreCase))
            {
                ApplyWarehousesExamples(operation, method, path);
            }
            // 6. Purchases / PurchaseReceipt
            else if (controller.Equals("purchases", StringComparison.OrdinalIgnoreCase) || controller.Equals("PurchaseReceipt", StringComparison.OrdinalIgnoreCase))
            {
                ApplyPurchasesExamples(operation, method, path);
            }
            // 7. Users
            else if (controller.Equals("Users", StringComparison.OrdinalIgnoreCase))
            {
                ApplyUsersExamples(operation, method, path);
            }
            // 8. Authentication
            else if (controller.Equals("Authentication", StringComparison.OrdinalIgnoreCase))
            {
                ApplyAuthenticationExamples(operation, method, path);
            }
            // 9. StockTransactions
            else if (controller.Equals("StockTransactions", StringComparison.OrdinalIgnoreCase))
            {
                ApplyStockTransactionsExamples(operation, method, path);
            }
        }

        private void SetRequestExample(OpenApiOperation operation, IOpenApiAny example)
        {
            if (operation.RequestBody != null && operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
            {
                mediaType.Example = example;
            }
        }

        private void SetResponseExample(OpenApiOperation operation, string statusCode, IOpenApiAny example)
        {
            if (!operation.Responses.TryGetValue(statusCode, out var response))
            {
                response = new OpenApiResponse { Description = statusCode == "201" ? "Created" : "OK" };
                operation.Responses[statusCode] = response;
            }

            if (!response.Content.TryGetValue("application/json", out var mediaType))
            {
                mediaType = new OpenApiMediaType();
                response.Content["application/json"] = mediaType;
            }

            mediaType.Example = example;
        }

        private void ApplyCategoriesExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["name"] = new OpenApiString("Điện thoại"),
                ["seoAlias"] = new OpenApiString("dien-thoai"),
                ["seoDescription"] = new OpenApiString("Các loại điện thoại thông minh"),
                ["sortOrder"] = new OpenApiInteger(1),
                ["parentId"] = new OpenApiNull()
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["name"] = new OpenApiString("Điện thoại"),
                ["seoAlias"] = new OpenApiString("dien-thoai"),
                ["seoDescription"] = new OpenApiString("Các loại điện thoại thông minh"),
                ["sortOrder"] = new OpenApiInteger(1),
                ["parentId"] = new OpenApiNull(),
                ["isDeleted"] = new OpenApiBoolean(false)
            };

            if (method == "POST")
            {
                SetRequestExample(operation, requestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/trash", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplyCustomersExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["fullName"] = new OpenApiString("Nguyễn Văn A"),
                ["phoneNumber"] = new OpenApiString("0987654321"),
                ["address"] = new OpenApiString("123 Đường Lê Lợi, Quận 1, TP.HCM"),
                ["email"] = new OpenApiString("nguyenvana@gmail.com")
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["fullName"] = new OpenApiString("Nguyễn Văn A"),
                ["phoneNumber"] = new OpenApiString("0987654321"),
                ["address"] = new OpenApiString("123 Đường Lê Lợi, Quận 1, TP.HCM"),
                ["email"] = new OpenApiString("nguyenvana@gmail.com"),
                ["isDeleted"] = new OpenApiBoolean(false)
            };

            if (method == "POST")
            {
                SetRequestExample(operation, requestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/trash", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplyProductsExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["name"] = new OpenApiString("iPhone 15 Pro Max 256GB"),
                ["description"] = new OpenApiString("Điện thoại di động iPhone 15 Pro Max bản 256GB"),
                ["categoryId"] = new OpenApiInteger(1),
                ["code"] = new OpenApiString("IP15PM256"),
                ["isActive"] = new OpenApiBoolean(true),
                ["sellingPrice"] = new OpenApiDouble(30000000.0),
                ["originalPrice"] = new OpenApiDouble(25000000.0),
                ["initialStock"] = new OpenApiInteger(50),
                ["sku"] = new OpenApiString("IMEI-123456789")
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["name"] = new OpenApiString("iPhone 15 Pro Max 256GB"),
                ["description"] = new OpenApiString("Điện thoại di động iPhone 15 Pro Max bản 256GB"),
                ["categoryId"] = new OpenApiInteger(1),
                ["code"] = new OpenApiString("IP15PM256"),
                ["isActive"] = new OpenApiBoolean(true),
                ["isDefault"] = new OpenApiBoolean(false),
                ["sellingPrice"] = new OpenApiDouble(30000000.0),
                ["originalPrice"] = new OpenApiDouble(25000000.0),
                ["quantity"] = new OpenApiInteger(50),
                ["imageUrl"] = new OpenApiString("/images/products/ip15.jpg"),
                ["warehouseLocation"] = new OpenApiString("Khu A, Kệ 1")
            };

            if (method == "POST")
            {
                SetRequestExample(operation, requestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/trash", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplySuppliersExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["supplierName"] = new OpenApiString("Công ty TNHH Thiết bị Số"),
                ["contactPerson"] = new OpenApiString("Trần Thanh B"),
                ["phone"] = new OpenApiString("0912345678"),
                ["address"] = new OpenApiString("456 Đường Nguyễn Huệ, Quận 1, TP.HCM"),
                ["email"] = new OpenApiString("contact@thietbiso.com"),
                ["isActive"] = new OpenApiBoolean(true)
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["supplierName"] = new OpenApiString("Công ty TNHH Thiết bị Số"),
                ["contactPerson"] = new OpenApiString("Trần Thanh B"),
                ["phone"] = new OpenApiString("0912345678"),
                ["address"] = new OpenApiString("456 Đường Nguyễn Huệ, Quận 1, TP.HCM"),
                ["email"] = new OpenApiString("contact@thietbiso.com"),
                ["isActive"] = new OpenApiBoolean(true),
                ["isDeleted"] = new OpenApiBoolean(false)
            };

            if (method == "POST")
            {
                SetRequestExample(operation, requestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/trash", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplyWarehousesExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["location"] = new OpenApiString("Kho Quận 7 - KCN Tân Thuận"),
                ["capacity"] = new OpenApiInteger(1000),
                ["email"] = new OpenApiString("khoq7@warehouse.com")
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["location"] = new OpenApiString("Kho Quận 7 - KCN Tân Thuận"),
                ["capacity"] = new OpenApiInteger(1000),
                ["email"] = new OpenApiString("khoq7@warehouse.com"),
                ["isDeleted"] = new OpenApiBoolean(false)
            };

            if (method == "POST")
            {
                SetRequestExample(operation, requestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/trash", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplyPurchasesExamples(OpenApiOperation operation, string method, string path)
        {
            // Exclude sub-endpoints (cancel, approve, confirm, convert-type) from showing the purchase receipt creation example
            if (path.EndsWith("/cancel", System.StringComparison.OrdinalIgnoreCase) || 
                path.EndsWith("/approve", System.StringComparison.OrdinalIgnoreCase) || 
                path.EndsWith("/confirm", System.StringComparison.OrdinalIgnoreCase) || 
                path.EndsWith("/convert-type", System.StringComparison.OrdinalIgnoreCase))
                return;

            var requestExample = new OpenApiObject
            {
                ["type"] = new OpenApiInteger(1), // 1: Nhập, 2: Xuất
                ["supplierId"] = new OpenApiInteger(101),
                ["customerId"] = new OpenApiNull(),
                ["warehouseId"] = new OpenApiInteger(1),
                ["supplierName"] = new OpenApiString("Công ty TNHH ABC"),
                ["customerName"] = new OpenApiNull(),
                ["receiptDate"] = new OpenApiString("2026-05-19T15:30:00.000Z"),
                ["referenceCode"] = new OpenApiString("HD-12345"),
                ["note"] = new OpenApiString("Nhập hàng đợt 1 tháng 5"),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["productId"] = new OpenApiInteger(12),
                        ["quantity"] = new OpenApiInteger(20),
                        ["unitCost"] = new OpenApiDouble(500000.0)
                    }
                }
            };

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
                ["totalAmount"] = new OpenApiDouble(10000000.0),
                ["createdBy"] = new OpenApiString("802f1afd-89f3-45bd-b785-19346929326b"),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["productId"] = new OpenApiInteger(12),
                        ["productVariantId"] = new OpenApiInteger(12),
                        ["quantity"] = new OpenApiInteger(20),
                        ["unitCost"] = new OpenApiDouble(500000.0),
                        ["totalPrice"] = new OpenApiDouble(10000000.0)
                    }
                }
            };

            if (method == "POST" || method == "PUT")
            {
                SetRequestExample(operation, requestExample);
            }

            if (method == "POST")
            {
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
            }
        }

        private void ApplyUsersExamples(OpenApiOperation operation, string method, string path)
        {
            var postRequestExample = new OpenApiObject
            {
                ["email"] = new OpenApiString("user1@warehouse.com"),
                ["phoneNumber"] = new OpenApiString("0987654321"),
                ["firstName"] = new OpenApiString("Anh"),
                ["lastName"] = new OpenApiString("Nguyen"),
                ["userName"] = new OpenApiString("user1"),
                ["password"] = new OpenApiString("Password123!")
            };

            var putRequestExample = new OpenApiObject
            {
                ["firstName"] = new OpenApiString("Anh"),
                ["lastName"] = new OpenApiString("Nguyen"),
                ["email"] = new OpenApiString("user1@warehouse.com"),
                ["phoneNumber"] = new OpenApiString("0987654321"),
                ["userName"] = new OpenApiString("user1")
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiString("802f1afd-89f3-45bd-b785-19346929326b"),
                ["email"] = new OpenApiString("user1@warehouse.com"),
                ["phoneNumber"] = new OpenApiString("0987654321"),
                ["firstName"] = new OpenApiString("Anh"),
                ["lastName"] = new OpenApiString("Nguyen"),
                ["userName"] = new OpenApiString("user1"),
                ["isActive"] = new OpenApiBoolean(true)
            };

            if (method == "POST")
            {
                SetRequestExample(operation, postRequestExample);
                SetResponseExample(operation, "201", responseExample);
            }
            else if (method == "PUT")
            {
                if (path.EndsWith("/roles", StringComparison.OrdinalIgnoreCase))
                {
                    var roleAssignExample = new OpenApiObject
                    {
                        ["roleNames"] = new OpenApiArray { new OpenApiString("Admin"), new OpenApiString("Staff") }
                    };
                    SetRequestExample(operation, roleAssignExample);
                }
                else
                {
                    SetRequestExample(operation, putRequestExample);
                }
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    if (path.EndsWith("/roles", StringComparison.OrdinalIgnoreCase))
                    {
                        SetResponseExample(operation, "200", new OpenApiArray { new OpenApiString("Admin"), new OpenApiString("Staff") });
                    }
                    else
                    {
                        SetResponseExample(operation, "200", responseExample);
                    }
                }
                else if (path.EndsWith("/all", StringComparison.OrdinalIgnoreCase))
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
            }
        }

        private void ApplyAuthenticationExamples(OpenApiOperation operation, string method, string path)
        {
            var loginRequest = new OpenApiObject
            {
                ["userName"] = new OpenApiString("admin"),
                ["password"] = new OpenApiString("Password123!")
            };

            var loginResponse = new OpenApiObject
            {
                ["userName"] = new OpenApiString("admin"),
                ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImFkbWluIiwiZXhwIjoxNzgxNjMyMjg4fQ.signature"),
                ["expiresIn"] = new OpenApiInteger(3600),
                ["refreshToken"] = new OpenApiString("d76c9e82-2ba9-4fca-8641-f09c2567dfaa")
            };

            var registerRequest = new OpenApiObject
            {
                ["userName"] = new OpenApiString("staff1"),
                ["email"] = new OpenApiString("staff1@warehouse.com"),
                ["password"] = new OpenApiString("Password123!"),
                ["firstName"] = new OpenApiString("Thanh"),
                ["lastName"] = new OpenApiString("Luan"),
                ["phoneNumber"] = new OpenApiString("0987654321")
            };

            var refreshTokenRequest = new OpenApiObject
            {
                ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImFkbWluIiwiZXhwIjoxNzgxNjMyMjg4fQ.signature"),
                ["refreshToken"] = new OpenApiString("d76c9e82-2ba9-4fca-8641-f09c2567dfaa")
            };

            var changePasswordRequest = new OpenApiObject
            {
                ["oldPassword"] = new OpenApiString("Password123!"),
                ["newPassword"] = new OpenApiString("NewPassword123!"),
                ["confirmPassword"] = new OpenApiString("NewPassword123!")
            };

            if (method == "POST")
            {
                if (path.EndsWith("/Login", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, loginRequest);
                    SetResponseExample(operation, "200", loginResponse);
                }
                else if (path.EndsWith("/Register", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, registerRequest);
                    SetResponseExample(operation, "200", loginResponse);
                }
                else if (path.EndsWith("/refresh-token", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, refreshTokenRequest);
                    SetResponseExample(operation, "200", loginResponse);
                }
                else if (path.EndsWith("/change-password", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, changePasswordRequest);
                }
            }
        }

        private void ApplyStockTransactionsExamples(OpenApiOperation operation, string method, string path)
        {
            var requestExample = new OpenApiObject
            {
                ["productId"] = new OpenApiInteger(1),
                ["productVariantId"] = new OpenApiInteger(1),
                ["warehouseId"] = new OpenApiInteger(1),
                ["quantityChange"] = new OpenApiInteger(20),
                ["transactionType"] = new OpenApiInteger(1), // 1: Nhập, 2: Xuất
                ["referenceType"] = new OpenApiInteger(1),
                ["referenceId"] = new OpenApiInteger(123),
                ["note"] = new OpenApiString("Nhập hàng đợt 1"),
                ["balanceAfter"] = new OpenApiInteger(70)
            };

            var responseExample = new OpenApiObject
            {
                ["id"] = new OpenApiInteger(1),
                ["productId"] = new OpenApiInteger(1),
                ["productVariantId"] = new OpenApiInteger(1),
                ["warehouseId"] = new OpenApiInteger(1),
                ["quantityChange"] = new OpenApiInteger(20),
                ["transactionType"] = new OpenApiInteger(1),
                ["referenceType"] = new OpenApiInteger(1),
                ["referenceId"] = new OpenApiInteger(123),
                ["balanceAfter"] = new OpenApiInteger(70),
                ["createDate"] = new OpenApiString("2026-06-12T13:00:00Z"),
                ["lastModifiedDate"] = new OpenApiString("2026-06-12T13:00:00Z"),
                ["isCanceled"] = new OpenApiBoolean(false),
                ["cancelReason"] = new OpenApiNull(),
                ["canceledDate"] = new OpenApiNull(),
                ["canceledBy"] = new OpenApiNull()
            };

            if (method == "POST")
            {
                if (path.EndsWith("/importData", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, new OpenApiArray { requestExample });
                    SetResponseExample(operation, "200", new OpenApiObject
                    {
                        ["count"] = new OpenApiInteger(1),
                        ["ids"] = new OpenApiArray { new OpenApiInteger(1) }
                    });
                }
                else if (path.EndsWith("/bulk-import-stock", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/bulk-export-stock", StringComparison.OrdinalIgnoreCase))
                {
                    SetRequestExample(operation, new OpenApiArray { requestExample });
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
                else
                {
                    SetRequestExample(operation, requestExample);
                    SetResponseExample(operation, "201", responseExample);
                }
            }
            else if (method == "PUT")
            {
                if (path.EndsWith("/cancel", StringComparison.OrdinalIgnoreCase))
                {
                    var cancelRequest = new OpenApiObject
                    {
                        ["cancelReason"] = new OpenApiString("Nhập sai số lượng"),
                        ["canceledBy"] = new OpenApiString("802f1afd-89f3-45bd-b785-19346929326b")
                    };
                    SetRequestExample(operation, cancelRequest);
                    SetResponseExample(operation, "200", responseExample);
                }
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    SetResponseExample(operation, "200", responseExample);
                }
                else if (path.EndsWith("/filter", StringComparison.OrdinalIgnoreCase))
                {
                    var paginationExample = new OpenApiObject
                    {
                        ["items"] = new OpenApiArray { responseExample },
                        ["totalRecords"] = new OpenApiInteger(1)
                    };
                    SetResponseExample(operation, "200", paginationExample);
                }
                else
                {
                    SetResponseExample(operation, "200", new OpenApiArray { responseExample });
                }
            }
        }
    }
}
