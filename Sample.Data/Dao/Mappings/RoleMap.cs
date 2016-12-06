using Microsoft.EntityFrameworkCore;

namespace Sample.Data.Dao.Mappings
{
    public class RoleMap : IEntityMap
    {
        public RoleMap(ModelBuilder builder)
        {
            builder.Entity<Role>(t =>
            {
                t.ToTable("Roles");

                t.HasKey(c => c.Id);
                t.Property(c => c.Name).HasMaxLength(50).IsRequired();
                t.HasMany(c => c.Users)
                    .WithOne(c => c.Role)
                    .HasForeignKey(c => c.RoleId);
            });
        }
    }
}
