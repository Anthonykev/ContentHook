using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContentHook.DAL.Entities;

namespace ContentHook.DAL.ORMapper
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Transcript> Transcripts => Set<Transcript>();
        public DbSet<Job> Jobs => Set<Job>();           

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transcript>(entity =>
            {
                entity.ToTable("Transcripts");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.UserId).IsRequired().HasMaxLength(200);
                entity.Property(x => x.Text).IsRequired().HasColumnType("text");
                entity.Property(x => x.Language).HasMaxLength(50);
                entity.Property(x => x.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Jobs");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.UserId).IsRequired().HasMaxLength(200);
                entity.Property(x => x.Platform).IsRequired().HasMaxLength(20);
                entity.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(500);  
                entity.Property(x => x.VideoStorageKey).IsRequired().HasMaxLength(1000);  
                entity.Property(x => x.Status)
                      .IsRequired()
                      .HasConversion<string>()
                      .HasMaxLength(20);
                entity.Property(x => x.ErrorMessage).HasColumnType("text");
                entity.Property(x => x.CreatedAt).IsRequired();
                entity.Property(x => x.UpdatedAt).IsRequired();
            });
        }
    }
}

