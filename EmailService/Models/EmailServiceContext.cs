using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace EmailService.Models;

public partial class EmailServiceEntities : DbContext
{
    public EmailServiceEntities()
    {
    }

    public EmailServiceEntities(DbContextOptions<EmailServiceEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<IncomingMessageRecepient> IncomingMessageRecepients { get; set; }

    public virtual DbSet<IncomingMessageText> IncomingMessageTexts { get; set; }

    public virtual DbSet<VwIncomingMessage> VwIncomingMessages { get; set; }

    public virtual DbSet<VwMessageQueue> VwMessageQueues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["EmailServiceEntities"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IncomingMessageRecepient>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK__Incoming__C5B196028AB4035E");

            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.EmailSentDateTime).HasColumnType("datetime");
            entity.Property(e => e.IncomingMessageId).HasColumnName("IncomingMessageID");
            entity.Property(e => e.IsBcc).HasColumnName("isBCC");
            entity.Property(e => e.IsCc).HasColumnName("isCC");
            entity.Property(e => e.NotifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.ReadDateTime).HasColumnType("datetime");
            entity.Property(e => e.RecepientEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<IncomingMessageText>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK__Incoming__C5B1960204776D18");

            entity.ToTable("IncomingMessageText");

            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.CreatedEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Creation).HasColumnType("datetime");
            entity.Property(e => e.MessageType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Parameters)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwIncomingMessage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_IncomingMessages");

            entity.Property(e => e.CreatedDate)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CreatedEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Creation).HasColumnType("datetime");
            entity.Property(e => e.IncomingMessageId).HasColumnName("IncomingMessageID");
            entity.Property(e => e.IsBcc).HasColumnName("isBCC");
            entity.Property(e => e.IsCc).HasColumnName("isCC");
            entity.Property(e => e.MessageType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NotifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.ReadDateTime).HasColumnType("datetime");
            entity.Property(e => e.RecepientEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Uid).HasColumnName("UID");
        });

        modelBuilder.Entity<VwMessageQueue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_MessageQueue");

            entity.Property(e => e.CreatedEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Creation).HasColumnType("datetime");
            entity.Property(e => e.IncomingMessageId).HasColumnName("IncomingMessageID");
            entity.Property(e => e.IsBcc).HasColumnName("isBCC");
            entity.Property(e => e.IsCc).HasColumnName("isCC");
            entity.Property(e => e.NotifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.ReadDateTime).HasColumnType("datetime");
            entity.Property(e => e.RecepientEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Uid).HasColumnName("UID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
