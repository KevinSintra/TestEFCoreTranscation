using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestEFCoreTranscation
{
    public class TestDBContext : DbContext
    {
        public DbSet<TestModel> TestModel { get; set; }
        public DbSet<Employee> Employee { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            // Logging in Entity Framework Core: https://www.entityframeworktutorial.net/efcore/logging-in-entityframework-core.aspx
            // How to create a LoggerFactory with a ConsoleLoggerProvider? https://stackoverflow.com/questions/53690820/how-to-create-a-loggerfactory-with-a-consoleloggerprovider

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Debug)
            );
            var loggerFactory = serviceCollection.BuildServiceProvider()
                .GetService<ILoggerFactory>();

            //optionsBuilder.UseSqlServer("Data Source=20.205.208.185;initial catalog=TestDatabase;User ID=sioteksql;Password=sql@20210809;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=kevinConsole");
            optionsBuilder.UseLoggerFactory(loggerFactory)  //tie-up DbContext with LoggerFactory object
            .EnableSensitiveDataLogging()
            //.UseSqlServer("server=localhost;database=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite");
            .UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

#else
            optionsBuilder.UseSqlServer("server=localhost;database=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestModel>(entity =>
            {
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.IsEnable).IsRequired();
                //entity.HasOne(d => d.Publisher)
                //  .WithMany(p => p.Books);
            });
        }
    }
}
