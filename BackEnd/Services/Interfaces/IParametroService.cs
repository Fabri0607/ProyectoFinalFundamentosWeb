using BackEnd.DTO;

namespace BackEnd.Services.Interfaces
{
    public interface IParametroService
    {

        List<ParametroDTO> GetParametros();
        ParametroDTO GetParametroById(int id);
        ParametroDTO AddParametro(ParametroDTO parametro);
        ParametroDTO UpdateParametro(ParametroDTO parametro);
        ParametroDTO DeleteParametro(int id);
    }
}
