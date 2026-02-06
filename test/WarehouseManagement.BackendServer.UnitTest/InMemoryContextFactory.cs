using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.UnitTest
{
    // Fake database for testing
    public class InMemoryContextFactory
    {
        public ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }

}
