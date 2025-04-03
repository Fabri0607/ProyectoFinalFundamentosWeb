using System;
using System.Collections.Generic;

namespace Entities.Entities;

public partial class Venta
{
    public int VentaId { get; set; }

    public DateTime FechaVenta { get; set; }

    public string? Notas { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string EstadoVenta { get; set; } = null!;

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();
}
