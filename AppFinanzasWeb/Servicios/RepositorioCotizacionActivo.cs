using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioCotizacionesActivos
    {
        Task<CotizacionActivo> GetCotizDolarTarjeta();
        Task<CotizacionActivo> GetUltimaCotizPorMoneda(int idActivo, string? tipo);
    }
    public class RepositorioCotizacionActivo : IRepositorioCotizacionesActivos
    {
        private readonly string connectionString;

        public RepositorioCotizacionActivo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<CotizacionActivo> GetCotizDolarTarjeta()
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<CotizacionActivo>(@"SELECT 
	                                                                                VALOR 
                                                                                FROM Cotizacion_Activo CA 
                                                                                INNER JOIN Dim_Activo A1 ON CA.IDACTIVOBASE = A1.IDACTIVO 
                                                                                INNER JOIN Dim_Activo A2 ON CA.IDACTIVOCOMP = A2.IDACTIVO 
                                                                                WHERE A1.SIMBOLO = 'USD' AND A2.SIMBOLO = 'ARS' AND CA.TIPO = 'TARJETA' 
                                                                                AND IDFECHA = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo)");
        }

        public async Task<CotizacionActivo> GetUltimaCotizPorMoneda(int idActivo, string? tipo)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        VALOR 
                        FROM Cotizacion_Activo 
                        WHERE IDACTIVOCOMP = @IdActivo 
                        AND IDFECHA = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo)";

            if (tipo != null) {
                sql = sql + " AND TIPO = @Tipo";
                return await connection.QueryFirstOrDefaultAsync<CotizacionActivo>(sql, new { idActivo, tipo });
            }

            return await connection.QueryFirstOrDefaultAsync<CotizacionActivo>(sql,new {idActivo});
        } 
    }
}
