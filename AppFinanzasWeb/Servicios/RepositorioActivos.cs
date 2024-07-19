using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Core.Pipeline;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioActivos
    {
        Task<IEnumerable<Activo>> ObtenerPorTipo(int idTipo);
        Task Crear(Activo activo);
        Task<Activo> ObtenerPorId(int id);
        Task Actualizar(Activo activo);
        Task Borrar(int id);
        Task<bool> EsUsado(int id);
    }
    public class RepositorioActivos : IRepositorioActivos
    {
        private readonly string connectionString;

        public RepositorioActivos(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Activo>> ObtenerPorTipo(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Activo>(@"
                                                                     SELECT idActivo Id, Simbolo SIMBOLO, Nombre ActivoNombre
                                                                     FROM Dim_Activo
                                                                     WHERE idTipoActivo = @Id",
                                                                     new { Id });
        }

        public async Task Crear(Activo activo)
        {
            activo.ESREFERENCIACOTIZ = false;
            activo.ESPRINCIPAL = false;

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"INSERT INTO Dim_Activo (idTipoActivo, nombre, simbolo, esPrincipal, 
                                            esReferencia) VALUES (@IDTIPOACTIVO, @ActivoNombre, @SIMBOLO, @ESPRINCIPAL,
                                            @ESREFERENCIACOTIZ)", activo);

        }

        public async Task<Activo> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Activo>(@"
                                                                     SELECT idActivo Id, Nombre ActivoNombre, SIMBOLO, IDTIPOACTIVO
                                                                     FROM Dim_Activo
                                                                     WHERE idActivo = @Id",
                                                                     new { Id });
        }

        public async Task Actualizar(Activo activo)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_Activo 
                                            SET Nombre = @ActivoNombre,
                                            Simbolo = @SIMBOLO
                                            WHERE idActivo = @Id", activo);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Dim_Activo WHERE idActivo = @Id", new { id });
        }

        public async Task<bool> EsUsado(int id)
        {
            using var connection = new SqlConnection(connectionString);


            var esUsado = await connection.QuerySingleAsync<int>
                                                ("sp_CheckActivoUso",
                                                new
                                                {
                                                    ActivoId = id
                                                },
                                                commandType: System.Data.CommandType.StoredProcedure);

            return Convert.ToBoolean(esUsado);
        }
    }
    
}
