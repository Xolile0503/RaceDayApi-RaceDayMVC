using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RaceDayAPI.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Organiser> Organisers => Set<Organiser>();

        public DbSet<Participant> Participants => Set<Participant>();

        public DbSet<RaceEvent> RaceEvents => Set<RaceEvent>();

        public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RaceEvent>()
                .HasOne(r => r.Organiser)
                .WithMany()
                .HasForeignKey(r => r.OrganiserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(r => r.Participant)
                .WithMany()
                .HasForeignKey(r => r.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(r => r.RaceEvent)
                .WithMany()
                .HasForeignKey(r => r.RaceEventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRegistration>()
                .HasIndex(r => new { r.ParticipantId, r.RaceEventId })
                .IsUnique();
        }
    }
}