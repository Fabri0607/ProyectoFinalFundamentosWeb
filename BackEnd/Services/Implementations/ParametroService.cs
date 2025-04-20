using BackEnd.DTO;
using BackEnd.Services.Interfaces;
using DAL.Interfaces;
using Entities.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackEnd.Services.Implementations
{
    public class ParametroService : IParametroService
    {
        ILogger<ParametroService> _logger;
        IUnidadDeTrabajo _unidadDeTrabajo;

        public ParametroService(IUnidadDeTrabajo unidad,
                                 ILogger<ParametroService> logger)
        {
            _unidadDeTrabajo = unidad;
            _logger = logger;
        }

        ParametroDTO Convertir(Parametro parametro)
        {
            return new ParametroDTO
            {
                ParametroId = parametro.ParametroId,
                Descripcion = parametro.Descripcion,
                Activo = parametro.Activo,
                Tipo = parametro.Tipo,
                Codigo = parametro.Codigo,
                Valor = parametro.Valor
            };
        }

        Parametro Convertir(ParametroDTO parametroDTO)
        {
            return new Parametro
            {
                ParametroId = parametroDTO.ParametroId,
                Descripcion = parametroDTO.Descripcion,
                Activo = parametroDTO.Activo,
                Tipo = parametroDTO.Tipo,
                Codigo = parametroDTO.Codigo,
                Valor = parametroDTO.Valor
            };
        }

        public ParametroDTO AddParametro(ParametroDTO parametroDTO)
        {
            try
            {
                _logger.LogError("Ingresa a AddParametro");
                _unidadDeTrabajo.ParametroDAL.Add(Convertir(parametroDTO));
                _unidadDeTrabajo.Complete();
                return parametroDTO;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public ParametroDTO DeleteParametro(int id)
        {
            var parametro = new Parametro { ParametroId = id };
            _unidadDeTrabajo.ParametroDAL.Remove(parametro);
            _unidadDeTrabajo.Complete();
            return Convertir(parametro);
        }

        public List<ParametroDTO> GetParametros()
        {
            var parametros = _unidadDeTrabajo.ParametroDAL.Get();
            List<ParametroDTO> parametroDTOs = new List<ParametroDTO>();
            foreach (var parametro in parametros)
            {
                parametroDTOs.Add(Convertir(parametro));
            }
            return parametroDTOs;
        }

        public ParametroDTO GetParametroById(int id)
        {
            var result = _unidadDeTrabajo.ParametroDAL.FindById(id);
            return Convertir(result);
        }

        public ParametroDTO UpdateParametro(ParametroDTO parametroDTO)
        {
            var entity = Convertir(parametroDTO);
            _unidadDeTrabajo.ParametroDAL.Update(entity);
            _unidadDeTrabajo.Complete();
            return parametroDTO;
        }
    }
}

