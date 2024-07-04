using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioActivos
    {
        Task<IEnumerable<Activo>> ObtenerPorTipo(int idTipo);
        
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
    }
}
