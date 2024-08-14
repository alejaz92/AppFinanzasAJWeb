using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using AppFinanzasWeb.ViewModels;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioClaseMovimientos
    {
        Task Actualizar(ClaseMovimiento claseMovimiento);
        Task Borrar(int id);
        Task Crear(ClaseMovimiento claseMovimiento);
        Task<bool> EsUsado(int id);
        Task<bool> Existe(string descripcion);
        Task<IEnumerable<ClaseMovimiento>> Obtener();
        Task<ClaseMovimiento> ObtenerPorDescripcion(string Descripcion);
        Task<ClaseMovimiento> ObtenerPorId(int Id);
    }

    public class RepositorioClaseMovimientos : IRepositorioClaseMovimientos
    {
        private readonly string connectionString;

        public RepositorioClaseMovimientos(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ClaseMovimiento>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ClaseMovimiento>(@"SELECT IDCLASEMOVIMIENTO Id, descripcion ClaseMovimientoNombre, ingegr 
            FROM Dim_ClaseMovimiento");
        }

        public async Task Crear(ClaseMovimiento claseMovimiento)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"INSERT INTO Dim_ClaseMovimiento(Descripcion, IngEgr) VALUES 
            (@ClaseMovimientoNombre, @IngEgr)", claseMovimiento);
        }

        public async Task Actualizar(ClaseMovimiento claseMovimiento)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_ClaseMovimiento 
                                            SET Descripcion = @ClaseMovimientoNombre
                                            WHERE idClaseMovimiento = @Id", claseMovimiento);
        }

        public async Task<ClaseMovimiento> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<ClaseMovimiento>(@"
                                                                     SELECT idClaseMovimiento Id, Descripcion ClaseMovimientoNombre, IngEgr
                                                                     FROM Dim_ClaseMovimiento
                                                                     WHERE idClaseMovimiento = @Id",
                                                                     new { Id });
        }

        public async Task<ClaseMovimiento> ObtenerPorDescripcion(string Descripcion)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<ClaseMovimiento>(@"
                                                                     SELECT idClaseMovimiento Id, Descripcion ClaseMovimientoNombre, IngEgr
                                                                     FROM Dim_ClaseMovimiento
                                                                     WHERE descripcion = @Descripcion",
                                                                     new { Descripcion });
        }

        public async Task<bool> Existe(string descripcion)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                    @"SELECT 1
                                    FROM Dim_ClaseMovimiento
                                    WHERE Descripcion = @Descripcion;",
                                    new { descripcion });
            return existe == 1;
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Dim_ClaseMovimiento WHERE idClaseMovimiento = @Id", new { id });
        }

        public async Task<bool> EsUsado(int id)
        {
            using var connection = new SqlConnection(connectionString);


            var esUsado = await connection.QuerySingleAsync<int>
                                                ("sp_CheckClaseMovimientoUso",
                                                new
                                                {
                                                    ClaseMovimientoId = id
                                                },
                                                commandType: System.Data.CommandType.StoredProcedure);

            return Convert.ToBoolean(esUsado);
        }
    }
}
