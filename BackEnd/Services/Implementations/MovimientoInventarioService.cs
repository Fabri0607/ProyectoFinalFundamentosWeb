using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;

namespace BackEnd.Services.Implementations
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;
        ILogger<MovimientoInventarioService> _logger;

        public MovimientoInventarioService(IUnidadDeTrabajo unidadDeTrabajo, ILogger<MovimientoInventarioService> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }

        // Método para convertir de entidad a DTO incluyendo propiedades de navegación
        private MovimientoInventarioDTO Convertir(MovimientoInventario movimiento, bool incluirInfoRelacionada = false)
        {
            var dto = new MovimientoInventarioDTO
            {
                MovimientoId = movimiento.MovimientoId,
                Cantidad = movimiento.Cantidad,
                FechaMovimiento = movimiento.FechaMovimiento,
                Notas = movimiento.Notas,
                ProductoId = movimiento.ProductoId,
                TipoMovimientoId = movimiento.TipoMovimientoId,
                Referencia = movimiento.Referencia
            };

            // Si se solicita incluir información relacionada
            if (incluirInfoRelacionada)
            {
                // Obtener nombre del producto
                var producto = _unidadDeTrabajo.ProductoDAL.FindById(movimiento.ProductoId);
                if (producto != null)
                {
                    dto.ProductoNombre = producto.Nombre;
                }

                // Obtener nombre del tipo de movimiento
                var tipoMovimiento = _unidadDeTrabajo.ParametroDAL.FindById(movimiento.TipoMovimientoId);
                if (tipoMovimiento != null)
                {
                    dto.TipoMovimientoNombre = tipoMovimiento.Valor;
                }
            }

            return dto;
        }

        // Método para convertir de DTO a entidad
        private MovimientoInventario Convertir(MovimientoInventarioDTO dto)
        {
            return new MovimientoInventario
            {
                MovimientoId = dto.MovimientoId,
                Cantidad = dto.Cantidad,
                FechaMovimiento = dto.FechaMovimiento,
                Notas = dto.Notas,
                ProductoId = dto.ProductoId,
                TipoMovimientoId = dto.TipoMovimientoId,
                Referencia = dto.Referencia
            };
        }

        public MovimientoInventarioDTO AddMovimiento(MovimientoInventarioDTO movimientoDTO)
        {
            try
            {
                _logger.LogInformation("Iniciando AddMovimiento");

                // Convertir DTO a entidad
                var movimiento = Convertir(movimientoDTO);
                movimiento.MovimientoId = 0; // Asegurar que sea un nuevo registro
                movimiento.FechaMovimiento = DateTime.Now; // Establecer fecha actual

                // Obtener el producto asociado
                var producto = _unidadDeTrabajo.ProductoDAL.FindById(movimiento.ProductoId);
                if (producto == null)
                {
                    _logger.LogError($"Producto no encontrado: {movimiento.ProductoId}");
                    throw new Exception($"No existe el producto con ID {movimiento.ProductoId}");
                }

                // Obtener el tipo de movimiento
                var tipoMovimiento = _unidadDeTrabajo.ParametroDAL.FindById(movimiento.TipoMovimientoId);

                if (tipoMovimiento == null)
                {
                    _logger.LogError($"No se encontró ningún parámetro con ID {movimiento.TipoMovimientoId}");
                    throw new Exception($"No existe un parámetro con ID {movimiento.TipoMovimientoId}");
                }

                if (tipoMovimiento.Tipo != "TIPO_MOVIMIENTO")
                {
                    _logger.LogError($"El parámetro con ID {movimiento.TipoMovimientoId} no es un tipo de movimiento válido. Tipo actual: {tipoMovimiento.Tipo}");
                    throw new Exception($"No existe un tipo de movimiento válido con ID {movimiento.TipoMovimientoId}");
                }

                // Actualizar el stock del producto según el tipo de movimiento
                if (tipoMovimiento.Codigo == "MOV_ENT")
                {
                    producto.Stock += movimiento.Cantidad;
                }
                else if (tipoMovimiento.Codigo == "MOV_SAL")
                {
                    // Verificar que haya stock suficiente
                    if (producto.Stock < movimiento.Cantidad)
                    {
                        _logger.LogError($"Stock insuficiente para el producto {producto.Nombre}");
                        throw new Exception($"Stock insuficiente para el producto {producto.Nombre}. Stock actual: {producto.Stock}, Cantidad solicitada: {movimiento.Cantidad}");
                    }
                    producto.Stock -= movimiento.Cantidad;
                }
                else if (tipoMovimiento.Codigo == "MOV_AJU")
                {
                    // Para ajustes, la cantidad puede ser positiva o negativa
                    producto.Stock = movimiento.Cantidad; // Establecer directamente el nuevo valor
                }

                // Guardar el movimiento
                _unidadDeTrabajo.MovimientoInventarioDAL.Add(movimiento);

                // Actualizar el producto
                producto.FechaModificacion = DateTime.Now;
                _unidadDeTrabajo.ProductoDAL.Update(producto);

                // Guardar todos los cambios en una transacción
                _unidadDeTrabajo.Complete();

                // Devolver el DTO con el ID generado y la información relacionada
                movimientoDTO.MovimientoId = movimiento.MovimientoId;

                // Establecer información adicional
                movimientoDTO.ProductoNombre = producto.Nombre;
                movimientoDTO.TipoMovimientoNombre = tipoMovimiento.Valor;

                return movimientoDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en AddMovimiento: {ex.Message}");
                throw;
            }
        }

        public List<MovimientoInventarioDTO> GetMovimientosByProducto(int productoId)
        {
            try
            {
                var movimientos = _unidadDeTrabajo.MovimientoInventarioDAL.Get()
                    .Where(m => m.ProductoId == productoId)
                    .OrderByDescending(m => m.FechaMovimiento)
                    .ToList();

                List<MovimientoInventarioDTO> resultados = new List<MovimientoInventarioDTO>();
                foreach (var item in movimientos)
                {
                    resultados.Add(Convertir(item, true));
                }

                return resultados;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener movimientos por producto: {ex.Message}");
                throw;
            }
        }

        public List<MovimientoInventarioDTO> GetAllMovimientos()
        {
            try
            {
                var movimientos = _unidadDeTrabajo.MovimientoInventarioDAL.Get()
                    .OrderByDescending(m => m.FechaMovimiento)
                    .ToList();

                List<MovimientoInventarioDTO> resultados = new List<MovimientoInventarioDTO>();
                foreach (var item in movimientos)
                {
                    resultados.Add(Convertir(item, true));
                }

                return resultados;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los movimientos: {ex.Message}");
                throw;
            }
        }

        public MovimientoInventarioDTO GetMovimientoById(int id)
        {
            try
            {
                var movimiento = _unidadDeTrabajo.MovimientoInventarioDAL.FindById(id);
                if (movimiento == null)
                {
                    throw new Exception($"No existe el movimiento con ID {id}");
                }

                return Convertir(movimiento, true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener movimiento por ID: {ex.Message}");
                throw;
            }
        }

        // No implementamos Update ni Delete porque generalmente los movimientos 
        // de inventario no deben modificarse una vez registrados
        // Si necesitas revertir un movimiento, se debería crear un nuevo movimiento en sentido contrario
    }
}
