﻿using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioMovTarjetas
    {
        Task CerrarRecurente(int id);
        Task InsertarMovimiento(MovTarjeta movTarjeta);
        Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina);
        Task<MovTarjeta> ObtenerPorId(int id);
        Task<int> ObtenerTotalMovimientos();
    }

    public class RepositorioMovTarjetas : IRepositorioMovTarjetas
    {
        private readonly string connectionString;

        public RepositorioMovTarjetas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
                            IdMovimiento,
	                        T1.FECHA FechaMov, 
	                        T.nombre NombreTarj, 
	                        CM.descripcion TipoMov, 
	                        FT.detalle Detalle, 
	                        CASE WHEN FT.repite = 'SI' THEN 'Recurrente' ELSE CAST(FT.cuotas AS varchar) END CuotaTexto ,
	                        A.nombre NombreMoneda, 
	                        FT.montoTotal MontoTotal,
	                        T2.FECHA MesPrimerCuota , 
	                        CASE WHEN repite = 'SI' THEN 'NA' ELSE CAST( T3.FECHA AS VARCHAR) END UltCuotaTexto, 
	                        FT.montoCuota MontoCuota
                        FROM [dbo].[Fact_Tarjetas2] FT 
                        INNER JOIN Dim_Tarjeta T ON T.idTarjeta = FT.idTarjeta 
                        INNER JOIN Dim_ClaseMovimiento CM ON CM.idClaseMovimiento = ft.idClaseMovimiento 
                        INNER JOIN Dim_Activo A ON A.idActivo = FT.idActivo 
                        INNER JOIN Dim_Tiempo T1 ON T1.IDFECHA = FT.FECHAMOV 
                        INNER JOIN Dim_Tiempo T2 ON T2.IDFECHA = FT.MESPRIMERCUOTA 
                        INNER JOIN Dim_Tiempo T3 ON T3.IDFECHA = FT.MESULTIMACUOTA 
                        WHERE FT.repite = 'SI' OR DATEADD(month ,1, T3.FECHA) >= GETDATE()
                        ORDER BY T1.fecha ASC
                        OFFSET @Offset ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY;";

            return await connection.QueryAsync<MovTarjeta>(sql, new
            {
                Offset = (pagina - 1) * cantidadPorPagina,
                cantidadPorPagina = cantidadPorPagina
            });
        }

        public async Task<int> ObtenerTotalMovimientos()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT COUNT(*) 
                        FROM [dbo].[Fact_Tarjetas2] FT INNER JOIN [dbo].[Dim_Tiempo] T ON FT.mesUltimaCuota = T.idFecha 
                        WHERE FT.repite = 'SI' OR DATEADD(month ,1, T.FECHA) >= GETDATE()";

            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task InsertarMovimiento(MovTarjeta movTarjeta)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"INSERT INTO [dbo].[Fact_Tarjetas2]
                           ([fechaMov]
                           ,[detalle]
                           ,[idTarjeta]
                           ,[idClaseMovimiento]
                           ,[idActivo]
                           ,[montoTotal]
                           ,[cuotas]
                           ,[mesPrimerCuota]
                           ,[mesUltimaCuota]
                           ,[repite]
                           ,[montoCuota])
                     VALUES
                           (@IdFecha
                           ,@Detalle
                           ,@IdTarjeta
                           ,@IdClaseMovimiento
                           ,@IdActivo
                           ,@MontoTotal
                           ,@Cuotas
                           ,@IdMesPrimerCuota
                           ,@IdMesUltimaCuota
                           ,@Repite
                           ,@MontoCuota);";

            await connection.ExecuteAsync(sql, movTarjeta);
        }

        public async Task<MovTarjeta> ObtenerPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT [idMovimiento]
                          ,TF.idFecha IdFecha
                          ,TF.[fecha] FechaMov
                          ,[detalle] Detalle
                          ,FT.idTarjeta IdTarjeta
                          ,FT.idClaseMovimiento IdClaseMovimiento
                          ,FT.idActivo IdActivo
                          ,[montoTotal] MontoTotal
                          ,[cuotas] Cuotas
                          ,[mesPrimerCuota] IdMesPrimerCuota
	                      ,T1.fecha MesPrimerCuota
                          ,[mesUltimaCuota] IdMesUltimaCuota
	                      , T2.fecha MesUltimaCuota
                          ,[repite] Repite
                          ,[montoCuota] MontoCuota
	                      , T.nombre NombreTarj
                      FROM [dbo].[Fact_Tarjetas2] FT
                      INNER JOIN [dbo].[Dim_Tarjeta] T ON T.idTarjeta = FT.idTarjeta
                      INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = FT.idClaseMovimiento
                      INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FT.idActivo
                      INNER JOIN [dbo].[Dim_Tiempo] TF ON TF.idFecha = FT.fechaMov
                      INNER JOIN [dbo].[Dim_Tiempo] T1 ON T1.idFecha = FT.mesPrimerCuota
                      INNER JOIN [dbo].[Dim_Tiempo] T2 ON T2.idFecha = FT.mesUltimaCuota
                      WHERE idMovimiento = @Id;";

            return await connection.QueryFirstOrDefaultAsync<MovTarjeta>(sql, new {id});
        }

        public async Task CerrarRecurente(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("UPDATE Fact_Tarjetas2 SET repite = 'Cerrado' WHERE IdMovimiento = @Id", new { id });
        }
    }
}
