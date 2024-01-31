using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace petChat.Database;

public partial class ChatDbContext : DbContext
{
    public ChatDbContext()
    {
    }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersChat> UsersChats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-CPI51I3\\MSSQLSERVER01;Initial Catalog=ChatDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat);

            entity.ToTable("Chat");

            entity.Property(e => e.IdChat).ValueGeneratedNever();
            entity.Property(e => e.Desription)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.IdMessage);

            entity.ToTable("Message");

            entity.Property(e => e.IdMessage).ValueGeneratedNever();
            entity.Property(e => e.Text)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(e => e.User).WithMany(e => e.Messages).HasForeignKey(e => e.IdUser);
            entity.HasOne(e => e.Chat).WithMany(e => e.Messages).HasForeignKey(e => e.IdChat);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(3000)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Nickname)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UsersChat>(entity =>
        {
            entity.HasKey(e => e.IdUsersChat);

            entity.ToTable("UsersChat");

            entity.Property(e => e.IdUsersChat).ValueGeneratedNever();

            entity.HasOne(e=>e.User).WithMany(e=>e.Chats).HasForeignKey(e=>e.IdChat);
            entity.HasOne(e => e.Chat).WithMany(e => e.Users).HasForeignKey(e => e.IdUser);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
