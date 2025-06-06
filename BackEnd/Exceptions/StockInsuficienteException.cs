namespace BackEnd.Exceptions
{
    public class StockInsuficienteException : Exception
    {
        public string ProductoNombre { get; }
        public int StockDisponible { get; }
        public int CantidadSolicitada { get; }

        public StockInsuficienteException(string productoNombre, int stockDisponible, int cantidadSolicitada)
            : base($"Stock insuficiente para el producto {productoNombre}. Disponible: {stockDisponible}, Solicitado: {cantidadSolicitada}")
        {
            ProductoNombre = productoNombre;
            StockDisponible = stockDisponible;
            CantidadSolicitada = cantidadSolicitada;
        }
    }
}
