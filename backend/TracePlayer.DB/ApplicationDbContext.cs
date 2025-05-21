using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TracePlayer.DB.Models;

namespace TracePlayer.DB
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        // dotnet ef migrations add InitialCreate -p TracePlayer.DB\TracePlayer.DB.csproj -s TracePlayer.API\TracePlayer.API.csproj

        // docker cp .\IP2LOCATION-LITE-DB1.CSV traceplayer-postgres:/tmp/IP2LOCATION-LITE-DB1.CSV
        // docker cp .\init_tran_hnsblock_for_pg.sql traceplayer-postgres:/init_tran_hnsblock_for_pg.sql
        // docker cp .\init_ip_countries_for_pg.sql traceplayer-postgres:/init_ip_countries_for_pg.sql
        // docker cp .\insert_player_info_pg.sql traceplayer-postgres:/insert_player_info_pg.sql
        // docker cp .\update_player_ips_pg.sql traceplayer-postgres:/update_player_ips_pg.sql
        // docker exec -it traceplayer-postgres psql -U postgres -d traceplayer -f /init_tran_hnsblock_for_pg.sql
        // docker exec -it traceplayer-postgres psql -U postgres -d traceplayer -f /init_ip_countries_for_pg.sql
        // docker exec -it traceplayer-postgres psql -U postgres -d traceplayer -f /insert_player_info_pg.sql
        // docker exec -it traceplayer-postgres psql -U postgres -d traceplayer -f /update_player_ips_pg.sql

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
        public DbSet<IpCountry> IpCountries { get; set; }
       
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

            modelBuilder.Entity<IpCountry>(entity =>
            {
                entity.ToTable("ip_country");

                entity.HasKey(e => new { e.IpFrom, e.IpTo });

                entity.Property(e => e.IpFrom)
                      .HasColumnName("ip_from")
                      .HasColumnType("bigint");

                entity.Property(e => e.IpTo)
                      .HasColumnName("ip_to")
                      .HasColumnType("bigint");

                entity.Property(e => e.CountryCode)
                      .HasColumnName("country_code")
                      .HasColumnType("char(2)")
                      .IsRequired();

                entity.Property(e => e.CountryName)
                      .HasColumnName("country_name")
                      .HasColumnType("text")
                      .IsRequired();
            });
        }
    }
}
