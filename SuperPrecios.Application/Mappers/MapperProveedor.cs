using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperProveedor
    {
        public static Proveedor ToProveedor(DtoProveedorUpdate dto)
        {
            var proveedor = new Proveedor(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password
            );
            proveedor.Id = dto.Id;
            return proveedor;
        }

        public static Proveedor ToProveedor(DtoProveedorAdd dto, Supermercado Supermercado)
        {            
            return new Proveedor(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password,
                Supermercado
            );
        }

        public static Proveedor ToProveedorWOPassword(DtoProveedorUpdate dto)
        {
            var proveedor = new Proveedor(
                dto.Nombre,
                dto.Apellido,
                dto.Email
            );
            proveedor.Id = dto.Id;
            return proveedor;
        }

        public static DtoProveedorGet ToDto(Proveedor proveedor)
        {
            return new DtoProveedorGet
            {
                Id = proveedor.Id,
                SupermercadoNombre = proveedor.Supermercado?.Nombre ?? "No asignado",
                Nombre = proveedor.Nombre,
                Apellido = proveedor.Apellido,
                Email = proveedor.Email.Valor
            };
        }

        public static DtoProveedorUpdate ToDtoUpdate(Proveedor proveedor)
        {
            return new DtoProveedorUpdate
            {
                Id = proveedor.Id,                 
                Nombre = proveedor.Nombre,
                Apellido = proveedor.Apellido,
                Email = proveedor.Email.Valor
            };
        }
        public static IEnumerable<DtoProveedorGet> ToDtoLista(IEnumerable<Proveedor> proveedores)
        {
            return proveedores.Select(p => ToDto(p));
        }
    }
}
