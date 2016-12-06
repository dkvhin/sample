using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Sample.Data.Tests
{
    public abstract class BaseRepositoryTest : IDisposable
    {
        protected IConfigurationRoot Configuration;
        protected IDatabaseContext Context;

        protected BaseRepositoryTest()
        {
            // Load the appsettings
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",
                    optional: true, reloadOnChange: true);
            Configuration = builder.Build();


            // create the option builder to set the connectionstring
            // we are using sql server for this
            var optionBuilder = new DbContextOptionsBuilder<SampleContext>();
            optionBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            // We will use sample context
            Context = new SampleContext(optionBuilder.Options);

            // we will set transaction to read uncommitted to read them back
            Context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
        }


        public void Dispose()
        {
            Context.Database.RollbackTransaction();
        }
    }
}
