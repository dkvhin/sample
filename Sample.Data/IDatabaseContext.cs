using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sample.Data.Dao;

namespace Sample.Data
{
    public interface IDatabaseContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DatabaseFacade Database { get; }
    }
}
