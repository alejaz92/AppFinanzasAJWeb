using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioTarjetas
    {
        Task<IEnumerable<Tarjeta>> Obtener();
        Task Crear(Tarjeta tarjeta);
        Task<Tarjeta> ObtenerPorId(int id);
        Task Actualizar(Tarjeta tarjeta);
        Task Borrar(int id);
        Task<bool> Existe(string nombre);
    }
    public class RepositorioTarjetas : IRepositorioTarjetas
    {
        private readonly string connectionString;

        public RepositorioTarjetas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Tarjeta>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Tarjeta>(@"SELECT IDTARJETA ID, NOMBRE FROM Dim_Tarjeta");
        }

        public async Task<Tarjeta> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Tarjeta>(@"
                                                                     SELECT idTarjeta Id, Nombre
                                                                     FROM Dim_Tarjeta
                                                                     WHERE idTarjeta = @Id",
                                                                     new { Id });
        }

        public async Task Actualizar(Tarjeta tarjeta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_Tarjeta
                                            SET Nombre = @Nombre
                                            WHERE idTarjeta = @Id", tarjeta);
        }

        public async Task Crear(Tarjeta tarjeta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"INSERT INTO Dim_Tarjeta(Nombre) VALUES (@Nombre)", tarjeta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Dim_Tarjeta WHERE idTarjeta = @Id", new { id });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                    @"SELECT 1
                                    FROM Dim_Tarjeta
                                    WHERE Nombre = @Nombre;",
                                    new { nombre });
            return existe == 1;
        }
    }
}
