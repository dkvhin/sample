using Microsoft.EntityFrameworkCore;

namespace Sample.Data.Dao.Mappings
{
    public class UserMap : IEntityMap
    {
        public UserMap(ModelBuilder builder)
        {
            builder.Entity<User>(t =>
            {
                t.ToTable("Users");

                t.HasKey(c => c.Id);
                t.Property(c => c.Username).HasMaxLength(15).IsRequired();
                t.Property(c => c.Password).HasMaxLength(255).IsRequired();
                t.Property(c => c.Email).HasMaxLength(255).IsRequired();
                t.HasOne(c => c.Role)
                    .WithMany(c => c.Users)
                    .HasForeignKey(c => c.RoleId);
            });
        }
    }
}
