using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Sample.DatabaseMigration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("DBMigration - Loaded Database Migration App.");
            Console.WriteLine("DBMigration - Loading AppSettings...");
            /*
             * Load the appsettings.json file to populate the necessary configuration values.
             */

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            builder.AddCommandLine(args);
            var config = builder.Build();

            var connectionstring = config.GetConnectionString("DefaultConnection");

            if (args.Length > 0)
                connectionstring = args[0];

            Console.WriteLine("DBMigration - AppSettings Loaded");

            Console.WriteLine("DBMigration - Using connectionstring : {0}", connectionstring);
            using (
                IDbConnection db = new SqlConnection(connectionstring)
                )
            {
                db.Open();

                using (IDbTransaction transaction = db.BeginTransaction())
                {
                    try
                    {
                        var repo = new MigrationRepository(db, transaction);

                        /*
                            * Check if the SchemaMigrations table is existing.
                            * This table is used for tracking the migration progress (base from the scripts filename).
                            */
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("DBMigration - Checking if SchemaMigrations table exist...");
                        if (!repo.IsSchemaTableExisting())
                        {
                            Console.WriteLine("DBMigration - SchemaMigrations table doesn't exist. Creating table...");

                            repo.CreateSchemaTable();

                            Console.WriteLine("DBMigration - SchemaMigrations table created.");
                        }

                        /*
                            * We then load all embedded sql script files under "Scripts" folder
                            * We are embeding them by adding the embed configuration under buildOptions in project.json
                            * file
                            */
                        var assembly = Assembly.GetEntryAssembly();
                        var resources =
                            assembly.GetManifestResourceNames().Where(c => c.Contains("Scripts"));
                        var myType = typeof(Program).Namespace;


                        /*
                            * We will wrapped everything in transaction just in case an error occurred we should be
                            * able to rollback.
                            */



                        Console.WriteLine("DBMigration - Begin Script Migration...");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        foreach (var resourceName in resources)
                        {
                            /*
                                * We need to remove the current namespace.
                                */
                            var name = resourceName.Replace(string.Format("{0}.Scripts.", myType), "");

                            /*
                                * Check if the script have been executed.
                                * If yes, then continue with the loop
                                */
                            if (repo.IsScriptExecuted(name))
                                continue;

                            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                var result = reader.ReadToEnd();

                                Console.WriteLine("DBMigration - Executing script : {0}", resourceName);

                                repo.ExecuteScript(name, result);

                                Console.WriteLine("DBMigration - Execution completed");
                            }
                        }

                        transaction.Commit();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DBMigration - Script Migration Completed.");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        transaction.Rollback();
                        Console.WriteLine("DBMigration - Script Migration Failed. Changes has been rolled back : {0}", ex);
                    }
                }
            }
        }
    }
}
