using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagement.BackendServer.Controllers
{
    [ApiController]
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Warehouse Management API</title>
    <!-- Bootstrap 5 CSS -->
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <!-- Google Fonts -->
    <link href=""https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;800&display=swap"" rel=""stylesheet"">
    <style>
        :root {
            --bg-color: #0b0f19;
            --card-bg: rgba(17, 25, 40, 0.65);
            --card-border: rgba(255, 255, 255, 0.08);
            --text-secondary: #94a3b8;
            --primary-glow: #6366f1;
            --success-glow: #10b981;
        }

        body {
            font-family: 'Outfit', sans-serif;
            background-color: var(--bg-color);
            color: #ffffff;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            position: relative;
        }

        /* Ambient Glow Backgrounds */
        body::before {
            content: '';
            position: absolute;
            width: 500px;
            height: 500px;
            background: radial-gradient(circle, rgba(99, 102, 241, 0.18) 0%, transparent 70%);
            top: -10%;
            left: -10%;
            z-index: 0;
        }

        body::after {
            content: '';
            position: absolute;
            width: 600px;
            height: 600px;
            background: radial-gradient(circle, rgba(16, 185, 129, 0.12) 0%, transparent 70%);
            bottom: -15%;
            right: -10%;
            z-index: 0;
        }

        .glass-card {
            background: var(--card-bg);
            backdrop-filter: blur(16px);
            -webkit-backdrop-filter: blur(16px);
            border: 1px solid var(--card-border);
            border-radius: 24px;
            box-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.37);
            animation: fadeIn 0.8s ease-out;
            z-index: 10;
            position: relative;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .logo-area {
            font-size: 3.5rem;
            animation: pulse 2.5s infinite;
        }

        @keyframes pulse {
            0%, 100% {
                transform: scale(1);
            }
            50% {
                transform: scale(1.05);
            }
        }

        h1 {
            font-weight: 800;
            background: linear-gradient(to right, #ffffff, #94a3b8);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            letter-spacing: -0.025em;
        }

        .status-badge {
            background: rgba(16, 185, 129, 0.1);
            border: 1px solid rgba(16, 185, 129, 0.2);
            color: var(--success-glow);
            padding: 0.5rem 1.25rem;
            border-radius: 50px;
            font-weight: 600;
            font-size: 0.875rem;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
        }

        .status-dot {
            width: 8px;
            height: 8px;
            background-color: var(--success-glow);
            border-radius: 50%;
            box-shadow: 0 0 12px var(--success-glow);
            animation: blink 1.5s infinite;
        }

        @keyframes blink {
            0%, 100% { opacity: 0.4; }
            50% { opacity: 1; }
        }

        .btn-custom {
            background: var(--primary-glow);
            color: #ffffff;
            font-weight: 600;
            border: none;
            padding: 0.85rem 2rem;
            border-radius: 12px;
            box-shadow: 0 4px 14px 0 rgba(99, 102, 241, 0.4);
            transition: all 0.3s ease;
        }

        .btn-custom:hover {
            background: #4f46e5;
            color: #ffffff;
            transform: translateY(-2px);
            box-shadow: 0 6px 20px 0 rgba(99, 102, 241, 0.6);
        }

        .footer-text {
            font-size: 0.75rem;
            color: rgba(255, 255, 255, 0.3);
            letter-spacing: 0.05em;
        }
    </style>
</head>
<body>
    <div class=""container d-flex flex-column align-items-center"">
        <div class=""glass-card p-5 text-center w-100"" style=""max-width: 500px;"">
            <div class=""logo-area mb-3"">🏭</div>
            <h1 class=""display-5 mb-2"">Warehouse API</h1>
            <p class=""text-muted mb-4 fw-light"">Warehouse Management Backend Service</p>
            <div class=""mb-4"">
                <span class=""status-badge"">
                    <span class=""status-dot""></span>
                    API ONLINE & READY
                </span>
            </div>
            <a href=""/swagger"" class=""btn btn-custom w-100 py-3"">Open API Documentation</a>
        </div>
        <div class=""footer-text mt-4 text-uppercase"">
            &copy; 2026 Warehouse Management System
        </div>
    </div>
    <!-- Bootstrap JS Bundle -->
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js""></script>
</body>
</html>";
            return new ContentResult
            {
                ContentType = "text/html",
                Content = html,
                StatusCode = 200
            };
        }
    }
}
