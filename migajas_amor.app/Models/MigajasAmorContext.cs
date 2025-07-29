using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace migajas_amor.app.Models;

public partial class MigajasAmorContext : DbContext
{
    public MigajasAmorContext()
    {
    }

    public MigajasAmorContext(DbContextOptions<MigajasAmorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolesasignado> Rolesasignados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

  /*  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=localhost;Database=migajas_amor;User=root;");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Rolesasignado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rolesasignados");

            entity.HasIndex(e => e.RolId, "RolId");

            entity.HasIndex(e => e.UsuarioId, "UsuarioId");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.Property(e => e.Login).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(64);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
