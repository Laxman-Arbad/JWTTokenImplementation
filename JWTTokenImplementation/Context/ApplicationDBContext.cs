using Microsoft.EntityFrameworkCore;
using JWTTokenImplementation.Models;
namespace JWTTokenImplementation.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :base(options)
        {

        }
        public virtual DbSet<Users> Users { get; set; }
    }
}
