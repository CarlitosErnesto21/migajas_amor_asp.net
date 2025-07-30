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

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolesasignado> Rolesasignados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("productos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(10)
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

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
