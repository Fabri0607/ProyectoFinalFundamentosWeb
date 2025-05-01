using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Entities;

public partial class SistemaInventarioVentasContext : DbContext
{
    public SistemaInventarioVentasContext()
    {
    }

    public SistemaInventarioVentasContext(DbContextOptions<SistemaInventarioVentasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

    public virtual DbSet<Parametro> Parametros { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //=> optionsBuilder.UseSqlServer("Server=PC-FABRI\\SQLEXPRESS;Database=SistemaInventarioVentas;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;");

    //Forma para conectarse de Nahum (NO BORRAR)
    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 => optionsBuilder.UseSqlServer("Server=192.168.0.13;Database=SistemaInventarioVentas;User Id=sa;Password=Mordecail#2014;TrustServerCertificate=True;");
    */
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PC-JUAND\\SQLEXPRESS;Database=SistemaInventarioVentas;Integrated Security=True;Trusted_Connection=True; TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.DetalleVentaId).HasName("PK__DetalleV__340EED44A950E7BB");

            entity.Property(e => e.DetalleVentaId).HasColumnName("DetalleVentaID");
            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VentaId).HasColumnName("VentaID");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Producto");

            entity.HasOne(d => d.Venta).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Venta");
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.MovimientoId).HasName("PK__Movimien__BF923FCC50B0FF86");

            entity.ToTable("MovimientoInventario");

            entity.Property(e => e.MovimientoId).HasColumnName("MovimientoID");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notas).HasColumnType("text");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoMovimientoId).HasColumnName("TipoMovimientoID");

            entity.HasOne(d => d.Producto).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimientoInventario_Producto");

            entity.HasOne(d => d.TipoMovimiento).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.TipoMovimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimientoInventario_TipoMovimiento");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.HasKey(e => e.ParametroId).HasName("PK__Parametr__2B3CE672CDC4EE4B");

            entity.ToTable("Parametro");

            entity.Property(e => e.ParametroId).HasColumnName("ParametroID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Valor)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83AE9EBC45");

            entity.ToTable("Producto");

            entity.HasIndex(e => e.Codigo, "UQ__Producto__06370DAC8761C956").IsUnique();

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PrecioCompra).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Parametro");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.VentaId).HasName("PK__Venta__5B41514C3B1A20D2");

            entity.HasIndex(e => e.NumeroFactura, "UQ__Venta__CF12F9A62805568D").IsUnique();

            entity.Property(e => e.VentaId).HasColumnName("VentaID");
            entity.Property(e => e.EstadoVenta)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notas).HasColumnType("text");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
