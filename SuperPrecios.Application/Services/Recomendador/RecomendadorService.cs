using SuperPrecios.Application.DTO.Recomendador;
using SuperPrecios.Application.IServices.Recomendador;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupermercadoCore = SuperPrecios.Domain.Entities.Supermercado;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;
using CarritoCore = SuperPrecios.Domain.Entities.Carrito;
using SuperPrecios.Domain.Entities.ValueObject;
using SuperPrecios.Application.Mappers;

namespace SuperPrecios.Application.Services.Recomendador
{
    public class RecomendadorService : IRecomendadorService
    {
        private readonly ICarritoRepository _carritoRepo;
        private readonly IPrecioHistoricoRepository _precioHistoricoRepo;
        public RecomendadorService(ICarritoRepository carritoRepo, IPrecioHistoricoRepository precioHistoricoRepo)
        {
            _carritoRepo = carritoRepo;
            _precioHistoricoRepo = precioHistoricoRepo;
        }

        public async Task<DtoBestPreciosSupermercado> GetRecomendacionCarrito(int idUsuario)
        {
            CarritoCore carritoUser = await GetCarritoUser(idUsuario);
            List<BestPreciosSupermercado> bpSupermercados = await GetBestPreciosSupermercados(carritoUser);
            BestPreciosSupermercado ganador = bpSupermercados.ElementAt(0);
            List<BestPreciosSupermercado> perdedores = new List<BestPreciosSupermercado>();
            if(bpSupermercados.Count > 1)
            {
                for (int i = 1; i < bpSupermercados.Count; i++)
                {
                    var bpSupermercado = bpSupermercados[i];
                    //Evaluar Empate
                    if (bpSupermercado.CostoTotalProductos < ganador.CostoTotalProductos)
                    {
                        perdedores.Add(ganador);
                        ganador = bpSupermercado;
                    }
                    else
                    {
                        perdedores.Add(bpSupermercado);
                    }
                }
            }
            
            DtoBestPreciosSupermercado dtoBPSupermercado = MapperSugerenciaService.ToDtoResultado(ganador, perdedores);
            return dtoBPSupermercado;
        }

        private async Task<List<BestPreciosSupermercado>> GetBestPreciosSupermercados(CarritoCore carrito)
        {            
            List<ProductoCore> productos = carrito.GetProductos();
            List<SupermercadoCore> supermercados = await _precioHistoricoRepo.GetSupermercadosFilterPreciosHistoricos(productos);
            if (supermercados == null || supermercados.Count <= 0) throw new Exception("No existen supermercados con esos productos");
            List<BestPreciosSupermercado> bestPreciosSupermercado = new List<BestPreciosSupermercado>();
            foreach(SupermercadoCore s in supermercados)
            {
                var precios = await _precioHistoricoRepo.GetPreciosHistoricosBySupermercado(s.Id, productos);
                List<LineaBestPrecios> lineasBP = MapperSugerenciaService.ToListLineaBestPrecios(carrito, precios);
                BestPreciosSupermercado bestPrecios = new BestPreciosSupermercado(s, lineasBP);
                bestPreciosSupermercado.Add(bestPrecios);
            }
            return bestPreciosSupermercado;
        }

        private async Task<CarritoCore> GetCarritoUser(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentException("Error con el usuario logueado");
            var carrito = await _carritoRepo.GetByUsuarioIdAsync(idUsuario);
            if (carrito == null || carrito.Lineas.Count <= 0) throw new ArgumentException("El carrito esta vacio, sin recomendaciones");
            return carrito;
        }
    }
}
