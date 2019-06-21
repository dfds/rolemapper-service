using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace K8sJanitor.WebApi.Infrastructure.Persistence
{
    public class K8sServiceDbContext : DbContext
    {
        public K8sServiceDbContext(DbContextOptions<K8sServiceDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<DomainEventEnvelope> DomainEvents { get; set; }
        
        // TODO Add relevant DBSets as well as OnModelCreating method for these.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DomainEventEnvelope>(cfg =>
            {
                cfg.HasKey(x => x.EventId);
                cfg.ToTable("DomainEvent");
            });
        }
    }
}