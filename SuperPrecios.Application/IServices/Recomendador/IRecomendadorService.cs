using SuperPrecios.Application.DTO.Recomendador;


namespace SuperPrecios.Application.IServices.Recomendador
{
    public interface IRecomendadorService
    {
        public Task<DtoBestPreciosSupermercado> GetRecomendacionCarrito(int idUsuario);
    }
}
