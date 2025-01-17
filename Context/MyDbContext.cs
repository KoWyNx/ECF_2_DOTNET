using System;
using System.Collections.Generic;
using EcfDotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace EcfDotnet.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Evenement> Evenements { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<RParticipantEvenement> RParticipantEvenements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=MyDbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evenement>(entity =>
        {
            entity.HasKey(e => e.Primarikey);

            entity.ToTable("EVENEMENT");

            entity.Property(e => e.Primarikey)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PRIMARIKEY");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("DATE_CREATION");
            entity.Property(e => e.DateDebut)
                .HasColumnType("datetime")
                .HasColumnName("DATE_DEBUT");
            entity.Property(e => e.DateFin)
                .HasColumnType("datetime")
                .HasColumnName("DATE_FIN");
            entity.Property(e => e.Localisation).HasColumnName("LOCALISATION");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.Primarikey);

            entity.ToTable("PARTICIPANT");

            entity.Property(e => e.Primarikey)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PRIMARIKEY");
            entity.Property(e => e.Age).HasColumnName("AGE");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("DATE_CREATION");
            entity.Property(e => e.Email).HasColumnName("EMAIL");
            entity.Property(e => e.Nom).HasColumnName("NOM");
            entity.Property(e => e.Prenom).HasColumnName("PRENOM");
            entity.Property(e => e.Telephone).HasColumnName("TELEPHONE");
        });

        modelBuilder.Entity<RParticipantEvenement>(entity =>
        {
            entity.HasKey(e => new { e.FkParticipant, e.FkEvenement });

            entity.ToTable("R_PARTICIPANT_EVENEMENT");

            entity.Property(e => e.FkParticipant).HasColumnName("FK_PARTICIPANT");
            entity.Property(e => e.FkEvenement).HasColumnName("FK_EVENEMENT");
            entity.Property(e => e.DateInscription)
                .HasColumnType("datetime")
                .HasColumnName("DATE_INSCRIPTION");

            entity.HasOne(d => d.FkEvenementNavigation).WithMany(p => p.RParticipantEvenements)
                .HasForeignKey(d => d.FkEvenement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_R_PARTICIPANT_EVENEMENT_EVENEMENT");

            entity.HasOne(d => d.FkParticipantNavigation).WithMany(p => p.RParticipantEvenements)
                .HasForeignKey(d => d.FkParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_R_PARTICIPANT_EVENEMENT_PARTICIPANT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
