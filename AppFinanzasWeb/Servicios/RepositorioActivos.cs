using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioActivos
    {
        Task<IEnumerable<Activo>> ObtenerPorTipo(int idTipo);
        Task Crear(Activo activo);
        Task<Activo> ObtenerPorId(int id);
        Task Actualizar(Activo activo);

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
                                                                     SELECT idActivo Id, Simbolo SIMBOLO, Nombre
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
                                            esReferencia) VALUES (@IDTIPOACTIVO, @Nombre, @SIMBOLO, @ESPRINCIPAL,
                                            @ESREFERENCIACOTIZ)", activo);

        }

        public async Task<Activo> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Activo>(@"
                                                                     SELECT idActivo Id, Nombre, SIMBOLO, IDTIPOACTIVO
                                                                     FROM Dim_Activo
                                                                     WHERE idActivo = @Id",
                                                                     new { Id });
        }

        public async Task Actualizar(Activo activo)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_Activo 
                                            SET Nombre = @Nombre,
                                            Simbolo = @SIMBOLO
                                            WHERE idActivo = @Id", activo);
        }
    }
    
}
