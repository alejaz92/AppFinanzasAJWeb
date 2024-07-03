using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioCuentas
    {
        Task<IEnumerable<Cuenta>> Obtener();
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id);
        Task Actualizar(Cuenta cuenta);
        Task Borrar(int id);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;
        
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"INSERT INTO Dim_Cuenta(Nombre) VALUES (@Nombre)", cuenta);
        }

        public async Task<IEnumerable<Cuenta>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@"SELECT IDCUENTA Id, NOMBRE FROM Dim_Cuenta");
        }

        public async Task<Cuenta> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"
                                                                     SELECT idCuenta Id, Nombre
                                                                     FROM Dim_Cuenta
                                                                     WHERE idCuenta = @Id",
                                                                     new { Id });
        }

        public async Task Actualizar(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_Cuenta 
                                            SET Nombre = @Nombre
                                            WHERE idCuenta = @Id", cuenta );
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection( connectionString);
            await connection.ExecuteAsync("DELETE FROM Dim_Cuenta WHERE idCuenta = @Id", new { id });
        }
    }
}
