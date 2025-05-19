using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TracePlayer.DB.Models;

namespace TracePlayer.DB
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        // dotnet ef migrations add InitialCreate -p TracePlayer.DB\TracePlayer.DB.csproj -s TracePlayer.API\TracePlayer.API.csproj

        // docker cp .\tran_hnsblock_for_pg.sql traceplayer-postgres:/tran_hnsblock_for_pg.sql
        // docker exec -it traceplayer-postgres psql -U postgres -d traceplayer -f /tran_hnsblock_for_pg.sql
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerName> PlayerNames { get; set; }
        public DbSet<PlayerIp> PlayerIps { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasMany(p => p.Names)
                      .WithOne(n => n.Player)
                      .HasForeignKey(n => n.PlayerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Ips)
                      .WithOne(n => n.Player)
                      .HasForeignKey(n => n.PlayerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlayerName>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.HasOne(n => n.Player)
                      .WithMany(p => p.Names)
                      .HasForeignKey(n => n.PlayerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlayerIp>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.HasOne(ip => ip.Player)
                      .WithMany(p => p.Ips)
                      .HasForeignKey(ip => ip.PlayerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiKey>()
                .HasIndex(k => k.ServerIp)
                .IsUnique();
        }
    }
}
