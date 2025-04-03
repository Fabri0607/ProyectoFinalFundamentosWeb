using System;
using System.Collections.Generic;

namespace Entities.Entities;

public partial class MovimientoInventario
{
    public int MovimientoId { get; set; }

    public int Cantidad { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? Notas { get; set; }

    public int ProductoId { get; set; }

    public int TipoMovimientoId { get; set; }

    public string? Referencia { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Parametro TipoMovimiento { get; set; } = null!;
}
