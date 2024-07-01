using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioCuentas
    {
        Task<IEnumerable<Cuenta>> Obtener();
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Cuenta>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@"SELECT IDCUENTA, NOMBRE FROM Dim_Cuenta");
        }
    }
}
