using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppFinanzasWeb.Servicios
{

    public interface IRepositorioTiposActivo
    {
        Task<bool> ActualizarCuentaTiposActivos(int IdCuenta, List<int> IdTipoActivos);
        Task<IEnumerable<TipoActivo>> Obtener();
        Task<IEnumerable<TipoActivo>> ObtenerBolsa();
        Task<IEnumerable<CuentaTipoActivo>> ObtenerPorCuenta(int IdCuenta);
        Task<TipoActivo> ObtenerPorId(int id);
    }
    public class RepositorioTiposActivo : IRepositorioTiposActivo
    {
        private readonly string connectionString;

        public RepositorioTiposActivo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TipoActivo>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoActivo>(@"SELECT IDTIPOACTIVO Id, Nombre FROM Dim_Tipo_Activo");
        }

        public async Task<TipoActivo> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<TipoActivo>(@"
                                                                     SELECT idTipoActivo Id, Nombre
                                                                     FROM Dim_Tipo_Activo
                                                                     WHERE idTipoActivo = @Id",
                                                                     new { Id });
        }

        public async Task<IEnumerable<TipoActivo>> ObtenerBolsa()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoActivo>(@"SELECT idTipoActivo Id, Nombre
                                                            FROM Dim_Tipo_Activo
                                                            WHERE Nombre NOT IN('Moneda', 'Criptomoneda')");
        }

        

        public async Task<IEnumerable<CuentaTipoActivo>> ObtenerPorCuenta(int IdCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<CuentaTipoActivo>(@"SELECT IdCuenta, CTA.idTipoActivo IdTipoActivo, Nombre 
                                                                                FROM [dbo].[Cuenta_TipoActivo] CTA
                                                                                INNER JOIN [dbo].[Dim_Tipo_Activo] TA ON TA.idTipoActivo = CTA.idTipoActivo
                                                                                WHERE CTA.IdCuenta = @IdCuenta",
                                                                                new { IdCuenta }); 
        }

        public async Task<bool> ActualizarCuentaTiposActivos(int IdCuenta, List<int> IdTipoActivos)
        {
            using var connection = new SqlConnection(connectionString);


            var tiposActuales = await ObtenerPorCuenta(IdCuenta);
            var tiposActualesIds = tiposActuales.Select(ta => ta.IdTipoActivo).ToList();
            
            var tiposDesasignados = tiposActualesIds.Except(IdTipoActivos).ToList();   
            
            if(await ExistenRegistrosAsociados(IdCuenta, tiposDesasignados))
            {
                return false;
            }

            connection.Open();

            var estado = connection.State;

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var deleteQuery = "DELETE FROM Cuenta_TipoActivo WHERE idCuenta = @IdCuenta";
                    await connection.ExecuteAsync(deleteQuery, new { IdCuenta = IdCuenta }, transaction);

                    foreach (var idTipoActivo in IdTipoActivos)
                    {
                        var insertQuery = "INSERT INTO Cuenta_TipoActivo (IdCuenta, IdTipoActivo) VALUES (@IdCuenta, @IdTipoActivo)";
                        await connection.ExecuteAsync(insertQuery, new { IdCuenta = IdCuenta, IdTipoActivo = idTipoActivo }, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback(); 
                    return false;
                }
            }
        }

        private async Task<bool> ExistenRegistrosAsociados(int IdCuenta, List<int>IdTipos)
        {
            using var connection = new SqlConnection(connectionString);

            var query = @"
            SELECT COUNT(*)
            FROM Fact_Movimiento FACT
            INNER JOIN Dim_Activo DIM ON DIM.idActivo = FACT.idActivo
            WHERE idCuenta = @IdCuenta AND idTipoActivo IN @Ids";

            var count = await connection.ExecuteScalarAsync<int>(query, new {IdCuenta = IdCuenta, Ids = IdTipos});

            return count > 0;
        }
    }
}
