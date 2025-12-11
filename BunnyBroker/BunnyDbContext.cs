using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BunnyBroker
{
    public class BunnyDbContext : DbContext
    {
	    public BunnyDbContext() : base() { }

        public BunnyDbContext(DbContextOptions<BunnyDbContext> options) : base(options)
	    {
	    }

	    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    optionsBuilder.AddInterceptors(new SaveBunnyMessageInterceptor());
	    }

        public DbSet<BunnyMessage> BunnyMessages { get; set; } = null!;
	    public DbSet<BunnyTypeRegistry> BunnyTypeRegistries { get; set; } = null!;
		public DbSet<BunnyLog> BunnyLogs { get; set; } = null!; 
		public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
	    {
			modelBuilder.Entity<BunnyLog>()
                .HasOne(b => b.BunnyTypeRegistry)
                .WithMany(x=>x.BunnyLogs)
                .HasForeignKey(x=>x.BunnyHandlerType);
			modelBuilder.Entity<BunnyLog>()
				.HasOne(b => b.BunnyMessage)
				.WithMany(x=>x.BunnyLogs)
				.HasForeignKey(x=>x.BunnyMessageId);

            base.OnModelCreating(modelBuilder);
	    }
    }
}
