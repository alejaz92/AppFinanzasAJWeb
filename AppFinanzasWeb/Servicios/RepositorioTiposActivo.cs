using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{

    public interface IRepositorioTiposActivo
    {
        Task<IEnumerable<TipoActivo>> Obtener();
        Task<Cuenta> ObtenerPorId(int id);
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

        public async Task<Cuenta> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"
                                                                     SELECT idTipoActivo Id, Nombre
                                                                     FROM Dim_Tipo_Activo
                                                                     WHERE idTipoActivo = @Id",
                                                                     new { Id });
        }
    }
}
