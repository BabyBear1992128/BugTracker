using BugTracker.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker
{
    public class MyContextFactory : IDbContextFactory<BugTrackerDb>
    {
        public BugTrackerDb Create()
        {
            return new BugTrackerDb(getConnectionString());
        }

        private string getConnectionString()
        {
            // DB Connect
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.IntegratedSecurity = true;
            builder.DataSource = "localhost";
            builder.UserID = "baby";
            builder.Password = "babybear";
            builder.InitialCatalog = "BugTracker";

            return builder.ConnectionString;
        }
    }

    public class BugTrackerDb : DbContext
    {
        public BugTrackerDb(string contextString) : base()
        {
            //Database.SetInitializer<BugTrackerDb>(new DropCreateDatabaseAlways<BugTrackerDb>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BugTrackerDb, Configuration>());
            this.Database.Connection.ConnectionString = contextString;
        }

        public DbSet<Bug> Bugs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Severity> Severities { get; set; }

    }
}
