# 🏭 Warehouse Management Backend

## 📌 Introduction

**Warehouse Management Backend** là hệ thống backend hỗ trợ quản lý kho hàng, bao gồm các chức năng như quản lý sản phẩm, nhập/xuất kho, tồn kho và các nghiệp vụ liên quan.

Dự án được xây dựng nhằm:

* Cung cấp API cho hệ thống quản lý kho
* Tối ưu quy trình nhập xuất và theo dõi tồn kho
* Làm nền tảng cho frontend (web/mobile)

---

## 🚀 Technologies

* .NET 8 (ASP.NET Core Web API)
* Entity Framework Core
* SQL Server
* JWT Authentication
* AutoMapper
* FluentValidation
* Logging (ILogger)

---

## 🧱 Architecture

Dự án được tổ chức theo mô hình **N-Tier Architecture** với cấu trúc chi tiết như sau:

```
WarehouseManagement-Backend
├── BackendServer
│   ├── Controllers          # Xử lý request/response
│   ├── Services             # Business logic
│   ├── Repositories         # Data access layer
│   ├── Entities             # Domain models (mapping DB)
│   ├── ViewModels           # DTOs trả về client
│   ├── Data
│   │   ├── DbContext        # Cấu hình EF Core
│   │   └── Migrations       # Migration files
│   ├── Configurations       # Fluent API config
│   ├── Validators           # FluentValidation
│   ├── Helpers              # Helper classes
│   ├── Constants            # Static constants
│   └── Program.cs           # Entry point
│
├── .gitignore
├── appsettings.json
└── README.md
```

### 📖 Giải thích:

* **Controllers** → nhận request từ client, trả response
* **Services** → xử lý nghiệp vụ (business logic)
* **Repositories** → thao tác với database (CRUD)
* **Entities** → đại diện bảng trong DB
* **ViewModels (DTOs)** → dữ liệu trao đổi giữa API và client
* **DbContext** → cấu hình kết nối DB và EF Core
* **Migrations** → quản lý version database
* **Configurations** → cấu hình quan hệ (Fluent API)
* **Validators** → validate dữ liệu đầu vào
* **Helpers** → các hàm tiện ích
* **Constants** → giá trị cố định toàn hệ thống

👉 Ưu điểm:

* Tách biệt rõ ràng từng layer
* Dễ maintain và test
* Dễ mở rộng (scalable)

---

## 🎮 Controllers Overview

# 📦 Tổng hợp các Controller đã thực hiện

Tài liệu này mô tả ngắn gọn các **Controller** đã được triển khai trong hệ thống Web API, kèm theo chức năng chính và tình trạng hiện tại.

---

## 🔐 AuthenticationController

**Commit:** `feat(auth): integrate JWT authentication for Web API`

* Xác thực người dùng bằng **JWT**
* Đăng nhập, tạo access token
* Phục vụ bảo mật cho toàn bộ API

---

## 🧱 BaseController

**Commit:** `feat(auth): integrate JWT authentication for Web API`

* Controller cơ sở dùng chung
* Tích hợp sẵn xác thực JWT
* Giảm lặp code cho các controller khác

---

## 🗂️ CategoriesController

**Commit:** `fix(categories): retrieve products from parent and child categories`

* Quản lý danh mục sản phẩm
* Lấy sản phẩm theo **danh mục cha và con**
* Hỗ trợ soft delete (trash)

---

## 📃 FunctionsController

**Commit:** `feat(controllers): add XML comments to CategoriesController`

* Chứa các chức năng hỗ trợ (helper / common functions)
* Bổ sung **XML comments** phục vụ Swagger & tài liệu API

---

## 🚀 PermissionsController

**Commit:** `feat(controllers): implement role permissions API`

* Quản lý quyền truy cập (permissions)
* Gán quyền cho role
* Phục vụ phân quyền hệ thống

---

## 📦 ProductsController

**Commit:** `feat(products): add XML documentation and logging support`

* CRUD sản phẩm
* Logging bằng `ILogger`
* XML documentation cho Swagger

---

## 🧪 RolePermissionsController

**Commit:** `feat(tests): add unit tests for CategoriesController`

* API trung gian giữa **Role** và **Permission**
* Hỗ trợ kiểm thử liên quan đến phân quyền

---

## 🧪 RolesController

**Commit:** `feat(tests): add unit tests for CategoriesController`

* Quản lý role (Admin, Staff, ...)
* Kết hợp với Permission để phân quyền

---

## 👤 UsersController

* Quản lý người dùng hệ thống
* CRUD user
* Gán role cho user

---

## 🔑 Features

* 🔐 Authentication & Authorization (JWT)
* 📦 Product Management
* 🏬 Warehouse Management
* 🔄 Stock Transaction (Import / Export)
* 📊 Inventory Tracking
* 🧾 Category Management
* ⚠️ Validation & Error Handling

---

## 📡 API Overview

### 🔑 Auth

* `POST /api/auth/login`
* `POST /api/auth/register`

### 📦 Product

* `GET /api/products`
* `POST /api/products`
* `PUT /api/products/{id}`
* `DELETE /api/products/{id}`

### 🏬 Warehouse

* `GET /api/warehouses`
* `POST /api/warehouses`

### 🔄 Stock Transaction

* Import / Export stock
* Track history

---

## ⚙️ Setup & Run

### 1️⃣ Clone project

```bash
git clone https://github.com/DOTHANHLUAN-24/WarehouseManagement-Backend.git
cd WarehouseManagement-Backend
```

### 2️⃣ Setup database

Update connection string trong `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "your_connection_string"
}
```

### 3️⃣ Apply migration

```bash
dotnet ef database update
```

### 4️⃣ Run project

```bash
dotnet run
```

---

## 🔐 Authentication

Hệ thống sử dụng JWT Token:

* Login → nhận token
* Gửi token qua header:

```
Authorization: Bearer {token}
```

---

## 🧪 Testing

* Sử dụng Postman / Swagger
* Swagger URL:

```
https://localhost:{port}/swagger
```

---

## 📊 Database Design

Hệ thống bao gồm các bảng chính:

* Products
* Categories
* Warehouses
* StockTransactions
* Users

---

## 📌 Future Improvements

* Pagination & Filtering
* Role-based Authorization
* Caching (Redis)
* Unit Test
* Docker Deployment

---

## 👨‍💻 Author

* DOTHANHLUAN

---

## 📄 License

This project is licensed under the MIT License.