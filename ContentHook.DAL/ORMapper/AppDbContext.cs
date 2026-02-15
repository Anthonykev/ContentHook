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
            : base(options)
        {
        }

        public DbSet<Transcript> Transcripts => Set<Transcript>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transcript>(entity =>
            {
                entity.ToTable("Transcripts");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.Text)
                      .IsRequired()
                      .HasColumnType("text");

                entity.Property(x => x.Language)
                      .HasMaxLength(50);

                entity.Property(x => x.CreatedAt)
                      .IsRequired();
            });
        }
    }
}
