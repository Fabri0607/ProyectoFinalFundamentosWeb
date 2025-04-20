namespace BackEnd.DTO
{
    public class ParametroDTO
    {
        public int ParametroId { get; set; }

        public string Descripcion { get; set; } = null!;

        public bool Activo { get; set; }

        public string Tipo { get; set; } = null!;

        public string Codigo { get; set; } = null!;

        public string? Valor { get; set; }
    }
}
