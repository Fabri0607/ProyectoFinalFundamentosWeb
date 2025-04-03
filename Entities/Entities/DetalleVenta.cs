using System;
using System.Collections.Generic;

namespace Entities.Entities;

public partial class DetalleVenta
{
    public int DetalleVentaId { get; set; }

    public int Cantidad { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
