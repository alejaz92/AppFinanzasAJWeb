using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data.SqlTypes;
using Microsoft.Identity.Client;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioMovimientos
    {
        Task Borrar(int id);
        Task InsertarMovimiento(Movimiento movimiento);
        Task<int> ObtenerIdMaximo();
        Task<IEnumerable<Movimiento>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina);
        Task<Movimiento> ObtenerPorId(int id);
        Task<int> ObtenerTotalMovimientos();
        Task Actualizar(int id, decimal monto);
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
                        FROM [dbo].[Fact_Movimiento2] MO 
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
            var sql = "SELECT COUNT(*) FROM Fact_Movimiento2 WHERE idClaseMovimiento IS NOT NULL";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> ObtenerIdMaximo()
        {
            using var conection = new SqlConnection(connectionString);

            var sql = "SELECT ISNULL(MAX(idMovimiento), 0) FROM Fact_Movimiento2";
            return await conection.ExecuteScalarAsync<int>(sql);
        }

        public async Task InsertarMovimiento(Movimiento movimiento)
        {
            using var connection = new SqlConnection(connectionString);

            string sqlPrecioCotiz;

            if (movimiento.PrecioCotiz!= 0)
            {
                if (movimiento.ActivoNombre == "Peso Argentino")
                {
                    sqlPrecioCotiz = movimiento.PrecioCotiz.ToString();
                }
                else
                {
                    sqlPrecioCotiz = Convert.ToString(1 / movimiento.PrecioCotiz);
                }
            }
            else
            {
                sqlPrecioCotiz = "(SELECT TOP 1 VALOR FROM [dbo].[Cotizacion_Activo] CA WHERE IDACTIVOCOMP = " +
                        "" + movimiento.IdActivo + " AND IDFECHA <= " + movimiento.Fecha.ToString("yyyyMMdd") + " " +
                        "ORDER BY idFecha DESC)";
            }

            string fecha = movimiento.Fecha.ToString("yyyyMMdd");

            var sql = "INSERT INTO Fact_Movimiento2 (IdMovimiento, idCuenta, idActivo, idFecha, tipoMovimiento, " +
                       " idClaseMovimiento, comentario, monto, precioCotiz) VALUES (@IdMovimiento, @IdCuenta, @IdActivo, " + fecha  + " , " + 
                       " @TipoMovimiento, @IdClaseMovimiento, @Comentario, @Monto, " + sqlPrecioCotiz + ")";

            await connection.ExecuteAsync(sql, movimiento);

        }
        
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Fact_Movimiento2 WHERE idMovimiento = @Id", new {id});

        }

        public async Task Actualizar(int id, decimal monto)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("UPDATE Fact_Movimiento2 SET monto = @Monto WHERE idMovimiento = @Id",
                    new { monto, id });
        }

        public async Task<Movimiento> ObtenerPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Movimiento>(@"SELECT  
	                                                                    FM.IdMovimiento IdMovimiento, 
	                                                                    C.IdCuenta IdCuenta,
	                                                                    C.nombre CuentaNombre,
	                                                                    A.idActivo IdActivo,
	                                                                    A.nombre ActivoNombre,
	                                                                    T.idFecha IdFecha,
	                                                                    T.fecha Fecha,
	                                                                    FM.tipoMovimiento TipoMovimiento,
	                                                                    FM.idClaseMovimiento IdClaseMovimiento,
	                                                                    CM.descripcion ClaseMovimientoNombre,
	                                                                    FM.comentario Comentario,
	                                                                    FM.Monto Monto, 
	                                                                    FM.precioCotiz PrecioCotiz
                                                                    FROM [dbo].[Fact_Movimiento2] FM
                                                                    INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                                                                    INNER JOIN [dbo].[Dim_Cuenta] C ON C.idCuenta = FM.idCuenta
                                                                    INNER JOIN [dbo].[Dim_Tiempo] T ON T.idFecha = FM.idFecha
                                                                    LEFT JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = FM.idClaseMovimiento
                                                                    WHERE FM.idMovimiento = @Id
                                                                    ", new { id });
            }
    }
}
