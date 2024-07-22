using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioMovimientos
    {
        Task<int> ObtenerIdMaximo();
        Task<IEnumerable<Movimiento>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina);
        Task<int> ObtenerTotalMovimientos();
    }

    public class RepositorioMovimientos : IRepositorioMovimientos
    {
        private readonly string connectionString;

        public RepositorioMovimientos(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Movimiento>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT  
	                        F.fecha Fecha, 
	                        MO.tipoMovimiento TipoMovimiento,
	                        CM.descripcion ClaseMovimientoNombre , 
	                        MO.comentario Comentario, 
	                        C.nombre CuentaNombre, 
	                        A.nombre ActivoNombre, 
	                        CAST(MO.monto AS decimal (18,2)) MONTO, 
	                        MO.IDMOVIMIENTO 
                        FROM [dbo].[Fact_Movimiento] MO 
                        INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = MO.idClaseMovimiento 
                        INNER JOIN Dim_Activo A ON A.idActivo = MO.idActivo 
                        INNER JOIN Dim_Cuenta C ON C.idCuenta = MO.idCuenta 
                        INNER JOIN Dim_Tiempo F ON MO.IDFECHA = F.IDFECHA 
                        ORDER BY MO.IDfecha DESC , idMovimiento DESC
                        OFFSET @Offset ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

            return await connection.QueryAsync<Movimiento>(sql, new
            {
                Offset = (pagina - 1) * cantidadPorPagina,
                cantidadPorPagina = cantidadPorPagina
            });
            
        }

        public async Task<int> ObtenerTotalMovimientos()
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM Fact_Movimiento WHERE idClaseMovimiento IS NOT NULL";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> ObtenerIdMaximo()
        {
            using var conection = new SqlConnection(connectionString);

            var sql = "SELECT ISNULL(MAX(idMovimiento), 0) FROM Fact_Movimiento";
            return await conection.ExecuteScalarAsync<int>(sql);
        }

        public async Task InsertarMovimiento(Movimiento movimiento)
        {
            using var connection  = new SqlConnection(connectionString);

            var sql = @"INSERT INTO Fact_Movimiento (IdMovimiento)"
        }
    }
}
