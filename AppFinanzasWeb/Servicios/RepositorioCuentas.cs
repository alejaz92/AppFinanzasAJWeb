using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioCuentas
    {
        Task<IEnumerable<Cuenta>> Obtener();
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id);
        Task Actualizar(Cuenta cuenta);
        Task Borrar(int id);
        Task<bool> Existe(string nombre);
        Task<bool> EsUsado(int id);
        Task<IEnumerable<Cuenta>> ObtenerPorTipo(string tipoActivo);
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
            await connection.ExecuteAsync(@"INSERT INTO Dim_Cuenta(Nombre) VALUES (@CuentaNombre)", cuenta);
        }

        public async Task<IEnumerable<Cuenta>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@"SELECT IDCUENTA Id, NOMBRE CuentaNombre FROM Dim_Cuenta");
        }

        public async Task<Cuenta> ObtenerPorId(int Id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"
                                                                     SELECT idCuenta Id, Nombre CuentaNombre
                                                                     FROM Dim_Cuenta
                                                                     WHERE idCuenta = @Id",
                                                                     new { Id });
        }

        public async Task Actualizar(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Dim_Cuenta 
                                            SET Nombre = @CuentaNombre
                                            WHERE idCuenta = @Id", cuenta );
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection( connectionString);
            await connection.ExecuteAsync("DELETE FROM Dim_Cuenta WHERE idCuenta = @Id", new { id });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                    @"SELECT 1
                                    FROM Dim_Cuenta
                                    WHERE Nombre = @Nombre;",
                                    new { nombre });
            return existe == 1;
        }

        public async Task<bool> EsUsado(int id)
        {
            using var connection = new SqlConnection(connectionString);


            var esUsado = await connection.QuerySingleAsync<int>
                                                ("sp_CheckCuentaUso",
                                                new
                                                {
                                                    CuentaId = id
                                                },
                                                commandType: System.Data.CommandType.StoredProcedure);

            return Convert.ToBoolean(esUsado);
        }

        public async Task<IEnumerable<Cuenta>> ObtenerPorTipo(string tipoActivo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@"
                                                                     SELECT C.IdCuenta Id, C.nombre CuentaNombre
                                                                    FROM [dbo].[Dim_Cuenta] C
                                                                    INNER JOIN [dbo].[Cuenta_TipoActivo] CTA ON C.idCuenta = CTA.IdCuenta
                                                                    INNER JOIN [dbo].[Dim_Tipo_Activo] TA ON TA.idTipoActivo = CTA.idTipoActivo
                                                                    WHERE TA.nombre = @tipoActivo",
                                                                     new { tipoActivo });
        }

    }
}
