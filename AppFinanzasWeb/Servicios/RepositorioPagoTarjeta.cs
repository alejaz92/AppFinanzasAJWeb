using AppFinanzasWeb.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{

    public interface IRepositorioPagosTarjeta
    {
        Task InsertarPago(PagoTarjeta pagoTarjeta);
    }
    public class RepositorioPagoTarjeta : IRepositorioPagosTarjeta
    {
        private readonly string connectionString;

        public RepositorioPagoTarjeta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InsertarPago(PagoTarjeta pagoTarjeta)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = @"INSERT INTO Pago_Tarjeta
                        (idTarjeta, fechaMes)
                        VALUES
                        (@IdTarjeta, @FechaMes);";
            
            await connection.ExecuteAsync(sql, pagoTarjeta);
        }
    }
}
