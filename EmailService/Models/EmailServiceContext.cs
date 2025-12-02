using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace EmailService.Models;

public partial class EmailServiceContext : DbContext
{
    public EmailServiceContext()
    {
    }

    public EmailServiceContext(DbContextOptions<EmailServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<IncomingMessageRecepient> IncomingMessageRecepients { get; set; }

    public virtual DbSet<IncomingMessageText> IncomingMessageTexts { get; set; }

    public virtual DbSet<VwIncomingMessage> VwIncomingMessages { get; set; }

    public virtual DbSet<VwMessageQueue> VwMessageQueues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(ConfigurationManager.ConnectionStrings["EmailServiceEntities"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<Credential>(entity =>
        {
            entity.ToTable("credentials");

            entity.HasKey(e => e.Uid).HasName("PK__Credenti__C5B19602CC1516BE");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.UserName).HasColumnName("username");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        modelBuilder.Entity<IncomingMessageRecepient>(entity =>
        {
            entity.ToTable("incomingmessagerecepients");

            entity.HasKey(e => e.Uid).HasName("PK__Incoming__C5B196028AB4035E");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.IncomingMessageId).HasColumnName("incomingmessageid");
            entity.Property(e => e.RecepientEmail).HasColumnName("recepientemail");
            entity.Property(e => e.IsCc).HasColumnName("iscc");
            entity.Property(e => e.IsBcc).HasColumnName("isbcc");
            entity.Property(e => e.IsNotified).HasColumnName("isnotified");
            entity.Property(e => e.NotifiedDateTime).HasColumnName("notifieddatetime");
            entity.Property(e => e.IsRead).HasColumnName("isread");
            entity.Property(e => e.ReadDateTime).HasColumnName("readdatetime");
            entity.Property(e => e.IsProcessed).HasColumnName("isprocessed");
            entity.Property(e => e.IsEmailSent).HasColumnName("isemailsent");
            entity.Property(e => e.EmailSentDateTime).HasColumnName("emailsentdatetime");
        });

        modelBuilder.Entity<IncomingMessageText>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK__Incoming__C5B1960204776D18");

            entity.ToTable("incomingmessagetext");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.BodyHtml).HasColumnName("bodyhtml");
            entity.Property(e => e.Creation).HasColumnName("creation");
            entity.Property(e => e.CreatedEmail).HasColumnName("createdemail");
            entity.Property(e => e.CreatedName).HasColumnName("createdname");
            entity.Property(e => e.IsEmail).HasColumnName("isemail");
            entity.Property(e => e.MessageType).HasColumnName("messagetype");
            entity.Property(e => e.Parameters).HasColumnName("parameters");
            entity.Property(e => e.IsSecure).HasColumnName("issecure");
            entity.Property(e => e.IsImportantTag).HasColumnName("isimportanttag");
        });

        modelBuilder.Entity<VwIncomingMessage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_incomingmessages");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.IncomingMessageId).HasColumnName("incomingmessageid");
            entity.Property(e => e.RecepientEmail).HasColumnName("recepientemail");
            entity.Property(e => e.IsCc).HasColumnName("iscc");
            entity.Property(e => e.IsBcc).HasColumnName("isbcc");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Creation).HasColumnName("creation");
            entity.Property(e => e.CreatedDate).HasColumnName("createddate");
            entity.Property(e => e.CreatedEmail).HasColumnName("createdemail");
            entity.Property(e => e.CreatedName).HasColumnName("createdname");
            entity.Property(e => e.IsProcessed).HasColumnName("isprocessed");
            entity.Property(e => e.IsEmailSent).HasColumnName("isemailsent");
            entity.Property(e => e.IsSecure).HasColumnName("issecure");
            entity.Property(e => e.ReadDateTime).HasColumnName("readdatetime");
            entity.Property(e => e.NotifiedDateTime).HasColumnName("notifieddatetime");
            entity.Property(e => e.BodyHtml).HasColumnName("bodyhtml");
            entity.Property(e => e.IsEmail).HasColumnName("isemail");
            entity.Property(e => e.MessageType).HasColumnName("messagetype");
            entity.Property(e => e.IsRead).HasColumnName("isread");
            entity.Property(e => e.IsImportantTag).HasColumnName("isimportanttag");
        });

        modelBuilder.Entity<VwMessageQueue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_messagequeue");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.IncomingMessageId).HasColumnName("incomingmessageid");
            entity.Property(e => e.RecepientEmail).HasColumnName("recepientemail");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.IsRead).HasColumnName("isread");
            entity.Property(e => e.Creation).HasColumnName("creation");
            entity.Property(e => e.CreatedEmail).HasColumnName("createdemail");
            entity.Property(e => e.CreatedName).HasColumnName("createdname");
            entity.Property(e => e.IsSecure).HasColumnName("issecure");
            entity.Property(e => e.NotifiedDateTime).HasColumnName("notifieddatetime");
            entity.Property(e => e.ReadDateTime).HasColumnName("readdatetime");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.BodyHtml).HasColumnName("bodyhtml");
            entity.Property(e => e.IsEmail).HasColumnName("isemail");
            entity.Property(e => e.IsCc).HasColumnName("iscc");
            entity.Property(e => e.IsBcc).HasColumnName("isbcc");
            entity.Property(e => e.IsImportantTag).HasColumnName("isimportanttag");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
