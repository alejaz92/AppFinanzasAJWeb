using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using AppFinanzasWeb.ViewModels.Estadistica;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioMovTarjetas
    {
        Task ActualizarRecurente(int id, decimal monto);
        Task CerrarRecurente(int id, int mesCierre);
        Task InsertarMovimiento(MovTarjeta movTarjeta);
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEstadisticaTarjetaMeses(string Tarjeta, string Moneda);
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEstadisticaTarjetaMesesTotal(string Moneda);
        Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina);
        Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPago(int IdTarjeta, string FechaPago);
        Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPagoTodas(string FechaPago);
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
                        FROM [dbo].[Fact_Tarjetas] FT 
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
                        FROM [dbo].[Fact_Tarjetas] FT INNER JOIN [dbo].[Dim_Tiempo] T ON FT.mesUltimaCuota = T.idFecha 
                        WHERE FT.repite = 'SI' OR DATEADD(month ,1, T.FECHA) >= GETDATE()";

            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task InsertarMovimiento(MovTarjeta movTarjeta)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"INSERT INTO [dbo].[Fact_Tarjetas]
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
                      FROM [dbo].[Fact_Tarjetas] FT
                      INNER JOIN [dbo].[Dim_Tarjeta] T ON T.idTarjeta = FT.idTarjeta
                      INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = FT.idClaseMovimiento
                      INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FT.idActivo
                      INNER JOIN [dbo].[Dim_Tiempo] TF ON TF.idFecha = FT.fechaMov
                      INNER JOIN [dbo].[Dim_Tiempo] T1 ON T1.idFecha = FT.mesPrimerCuota
                      INNER JOIN [dbo].[Dim_Tiempo] T2 ON T2.idFecha = FT.mesUltimaCuota
                      WHERE idMovimiento = @Id;";

            return await connection.QueryFirstOrDefaultAsync<MovTarjeta>(sql, new {id});
        }

        public async Task CerrarRecurente(int id, int mesCierre)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("UPDATE Fact_Tarjetas SET repite = 'Cerrado', mesUltimaCuota = @MesCierre WHERE IdMovimiento = @Id", new { id, mesCierre });
        }

        public async Task ActualizarRecurente(int id, decimal monto)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("UPDATE Fact_Tarjetas SET montoTotal = @Monto, montoCuota = @Monto  WHERE IdMovimiento = @Id", new { id, monto });
        }

        public async Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPago(int IdTarjeta, string FechaPago)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
                            A.IdActivo IdActivo,
                            TA.nombre NombreTarj,
	                        T1.FECHA FechaMov,
                            CM.IdClaseMovimiento IdClaseMovimiento,
	                        CM.descripcion TipoMov,
	                        T.detalle Detalle, 
	                        A.nombre NombreMoneda,
	                        CASE WHEN T.repite = 'SI' THEN 'Recurrente' ELSE CAST(DATEDIFF(MONTH, T2.FECHA, @FechaPago) + 1 AS VARCHAR) 
		                        + '/' + cast(T.cuotas as varchar) END CuotaTexto,
	                        T.montoCuota MontoCuota, 
	                        CAST(CASE WHEN A.NOMBRE = 'Peso Argentino' THEN T.montoCuota ELSE (T.montoCuota * CA.VALOR)  
		                        END  AS DECIMAL (18, 2)) ValorPesos 
                        FROM [dbo].[Fact_Tarjetas] T 
                        INNER JOIN [dbo].[Dim_Activo] A ON T.idActivo = A.idActivo 
                        INNER JOIN [dbo].[Cotizacion_Activo] CA ON CA.idActivoComp = (SELECT idActivo FROM Dim_Activo WHERE simbolo = 'ARS') AND 
	                        CA. TIPO = 'TARJETA' AND CA.IDFECHA = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) 
                        INNER JOIN Dim_ClaseMovimiento CM ON CM.idClaseMovimiento = T.idClaseMovimiento 
                        INNER JOIN [dbo].[Dim_Tarjeta] TA ON T.idTarjeta = TA.idTarjeta  
                        LEFT JOIN Pago_Tarjeta PT ON PT.idTarjeta = T.idTarjeta AND PT.fechaMes = REPLACE(@FechaPago, '-','') 
                        INNER JOIN Dim_Tiempo T1 ON   T1.IDFECHA = T.FECHAMOV 
                        INNER JOIN Dim_Tiempo T2 ON T2.IDFECHA = T.MESPRIMERCUOTA 
                        INNER JOIN    Dim_Tiempo T3 ON T3.IDFECHA = T.MESULTIMACUOTA WHERE  (T3.FECHA >= @FechaPago OR T.repite = 'SI') AND 
	                        T2.FECHA <= @FechaPago AND  TA.IdTarjeta = @IdTarjeta AND PT.idTarjeta IS NULL ORDER BY NombreMoneda, fechaMov;";


            return await connection.QueryAsync<MovTarjeta>(sql, new
            {
                IdTarjeta = IdTarjeta,
                FechaPago = FechaPago
            });

        }

        public async Task<IEnumerable<MovTarjeta>> ObtenerMovimientosPagoTodas(string FechaPago)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
                            A.IdActivo IdActivo,
                            TA.nombre NombreTarj,
	                        T1.FECHA FechaMov,
                            CM.IdClaseMovimiento IdClaseMovimiento,
	                        CM.descripcion TipoMov,
	                        T.detalle Detalle, 
	                        A.nombre NombreMoneda,
	                        CASE WHEN T.repite = 'SI' THEN 'Recurrente' ELSE CAST(DATEDIFF(MONTH, T2.FECHA, @FechaPago) + 1 AS VARCHAR) 
		                        + '/' + cast(T.cuotas as varchar) END CuotaTexto,
	                        T.montoCuota MontoCuota, 
	                        CAST(CASE WHEN A.NOMBRE = 'Peso Argentino' THEN T.montoCuota ELSE (T.montoCuota * CA.VALOR)  
		                        END  AS DECIMAL (18, 2)) ValorPesos 
                        FROM [dbo].[Fact_Tarjetas] T 
                        INNER JOIN [dbo].[Dim_Activo] A ON T.idActivo = A.idActivo 
                        INNER JOIN [dbo].[Cotizacion_Activo] CA ON CA.idActivoComp = (SELECT idActivo FROM Dim_Activo WHERE simbolo = 'ARS') AND 
	                        CA. TIPO = 'TARJETA' AND CA.IDFECHA = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) 
                        INNER JOIN Dim_ClaseMovimiento CM ON CM.idClaseMovimiento = T.idClaseMovimiento 
                        INNER JOIN [dbo].[Dim_Tarjeta] TA ON T.idTarjeta = TA.idTarjeta  
                        LEFT JOIN Pago_Tarjeta PT ON PT.idTarjeta = T.idTarjeta AND PT.fechaMes = REPLACE(@FechaPago, '-','') 
                        INNER JOIN Dim_Tiempo T1 ON   T1.IDFECHA = T.FECHAMOV 
                        INNER JOIN Dim_Tiempo T2 ON T2.IDFECHA = T.MESPRIMERCUOTA 
                        INNER JOIN    Dim_Tiempo T3 ON T3.IDFECHA = T.MESULTIMACUOTA WHERE  (T3.FECHA >= @FechaPago OR T.repite = 'SI') AND 
	                        T2.FECHA <= @FechaPago  AND PT.idTarjeta IS NULL ORDER BY NombreTarj,NombreMoneda, fechaMov;";


            return await connection.QueryAsync<MovTarjeta>(sql, new
            {
                FechaPago = FechaPago
            });

        }

        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEstadisticaTarjetaMeses(string Tarjeta, string Moneda)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
                        T.anio
                        ,T.mes
                        ,T.mesNombre mesNombre
                        ,COALESCE(SUM( montoCuota), 0) Total
                        FROM
                        [dbo].[Dim_Tiempo] T
                        LEFT JOIN
                        (
                        SELECT 
                        FT.*,
                        CAST(REPLACE(CAST(DATEADD(MONTH, n.Number, T1.fecha) AS NVARCHAR), '-', '') AS INT) as mes 
                        FROM [dbo].[Fact_Tarjetas] FT
                        INNER JOIN [dbo].[Dim_Tiempo] T1 ON T1.idFecha = FT.mesPrimerCuota
                        INNER JOIN [dbo].[Dim_Tiempo] T2 ON T2.idFecha = FT.mesUltimaCuota
                        INNER JOIN Dim_Tarjeta TA ON TA.idTarjeta = FT.idTarjeta AND TA.IdTarjeta  = @Tarjeta INNER 
                        JOIN Dim_Activo A ON A.idActivo = FT.idActivo AND A.nombre = @Moneda JOIN 
                        (
	                        SELECT ROW_NUMBER() OVER(ORDER BY object_id) - 1 AS Number
	                        FROM sys.objects) as n
	                        ON  DATEADD(MONTH, N.NUMBER, T1.FECHA) <= T2.fecha  OR ft.mesUltimaCuota = 0
                        ) T1 ON T.idFecha = T1.mes
                        WHERE T.fecha BETWEEN DATEADD(MONTH, -6, GETDATE()) AND DATEADD(MONTH, 5, GETDATE())
                        GROUP BY T.anio, T.mes, T.mesNombre
                        ORDER BY T.anio, T.mes;";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql, new { Tarjeta, Moneda});
        }

        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEstadisticaTarjetaMesesTotal(string Moneda)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
                        T.anio
                        ,T.mes
                        ,T.mesNombre mesNombre
                        ,COALESCE(SUM( montoCuota), 0) Total
                        FROM
                        [dbo].[Dim_Tiempo] T
                        LEFT JOIN
                        (
                        SELECT 
                        FT.*,
                        CAST(REPLACE(CAST(DATEADD(MONTH, n.Number, T1.fecha) AS NVARCHAR), '-', '') AS INT) as mes 
                        FROM [dbo].[Fact_Tarjetas] FT
                        INNER JOIN [dbo].[Dim_Tiempo] T1 ON T1.idFecha = FT.mesPrimerCuota
                        INNER JOIN [dbo].[Dim_Tiempo] T2 ON T2.idFecha = FT.mesUltimaCuota            
                        INNER JOIN Dim_Activo A ON A.idActivo = FT.idActivo AND A.nombre = @Moneda JOIN 
                        (
	                        SELECT ROW_NUMBER() OVER(ORDER BY object_id) - 1 AS Number
	                        FROM sys.objects) as n
	                        ON  DATEADD(MONTH, N.NUMBER, T1.FECHA) <= T2.fecha  OR ft.mesUltimaCuota = 0
                        ) T1 ON T.idFecha = T1.mes
                        WHERE T.fecha BETWEEN DATEADD(MONTH, -6, GETDATE()) AND DATEADD(MONTH, 5, GETDATE())
                        GROUP BY T.anio, T.mes, T.mesNombre
                        ORDER BY T.anio, T.mes;";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql, new { Moneda });
        }
    }
}
