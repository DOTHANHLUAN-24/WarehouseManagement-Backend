using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.UnitTest
{
    // Fake database for testing
    public class InMemoryContextFactory
    {
        public ApplicationDbContext GetApplicationDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryApplicationDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);

            return dbContext;
           
        }
    }
}
