using System;
using System.Collections.Generic;

namespace Entities.Entities;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public bool Activo { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int CategoriaId { get; set; }

    public virtual Parametro Categoria { get; set; } = null!;

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();
}
