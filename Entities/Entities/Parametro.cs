using System;
using System.Collections.Generic;

namespace Entities.Entities;

public partial class Parametro
{
    public int ParametroId { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public string Tipo { get; set; } = null!;

    public string Codigo { get; set; } = null!;

    public string? Valor { get; set; }

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
