using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEFCoreTranscation
{
    public class TestDBContext : DbContext
    {
        public DbSet<TestModel> TestModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("server=localhost;database=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite");
            optionsBuilder.UseSqlServer("Data Source=20.205.208.185;initial catalog=TestDatabase;User ID=sioteksql;Password=sql@20210809;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=kevinConsole");
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
