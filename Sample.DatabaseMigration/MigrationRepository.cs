using System;
using System.Data;
using Dapper;

namespace Sample.DatabaseMigration
{
    public class MigrationRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _dbTransaction;

        public MigrationRepository(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            _dbConnection = dbConnection;
            _dbTransaction = dbTransaction;
        }


        public bool IsSchemaTableExisting()
        {
            return _dbConnection.QueryFirst<bool>(
                "IF EXISTS (SELECT * FROM sys.tables WHERE Name = 'SchemaMigrations') SELECT 1 ELSE SELECT 0",
                transaction: _dbTransaction);
        }

        public void CreateSchemaTable()
        {
            _dbConnection.Execute(@"CREATE TABLE [dbo].[SchemaMigrations](
	                                [Id] [int] IDENTITY(1,1) NOT NULL,
	                                [Name] [varchar](max) NOT NULL,
	                                [DateExecuted] [datetime] NOT NULL,
                                 CONSTRAINT [PK_SchemaMigrations] PRIMARY KEY CLUSTERED 
                                (
	                                [Id] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", transaction: _dbTransaction);
        }

        public bool IsScriptExecuted(string name)
        {
            return _dbConnection.QueryFirst<bool>(
                "IF EXISTS (SELECT TOP 1 1 FROM SchemaMigrations WHERE Name = @name) SELECT 1 ELSE SELECT 0",
                new {name}, transaction: _dbTransaction);
        }

        public void ExecuteScript(string scriptName, string query)
        {
            _dbConnection.Execute(query, null, _dbTransaction);

            _dbConnection.Execute(@"INSERT INTO [dbo].[SchemaMigrations]
                                        ([Name]
                                        ,[DateExecuted])
                                    VALUES
                                        (@name
                                        ,@date)", new {name = scriptName, date = DateTime.Now}, _dbTransaction);

        }
    }
}
