using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sample.Data.Dao;
using Sample.Data.Dao.Mappings;

namespace Sample.Data
{
    public  sealed  class SampleContext : DbContext, IDatabaseContext
    {
        public SampleContext(DbContextOptions<SampleContext> options)
            : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // instead of adding every model configuration here
            // we wil be using reflection to separate them into classes
            RegisterMaps(modelBuilder);

            base.OnModelCreating(modelBuilder);
           
        }

        private void RegisterMaps(ModelBuilder builder)
        {
            var maps = Assembly.GetEntryAssembly().GetTypes()
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace)
                    && typeof(IEntityMap).IsAssignableFrom(type) && type.GetTypeInfo().IsClass).ToList();

            foreach (var item in maps)
                Activator.CreateInstance(item, BindingFlags.Public |
                BindingFlags.Instance, null, new object[] { builder }, null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
