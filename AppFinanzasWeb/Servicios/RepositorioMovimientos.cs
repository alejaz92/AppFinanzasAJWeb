using Dapper;
using AppFinanzasWeb.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data.SqlTypes;
using Microsoft.Identity.Client;
using System.Globalization;
using AppFinanzasWeb.Models.DTO;
using AppFinanzasWeb.ViewModels.Estadistica;

namespace AppFinanzasWeb.Servicios
{
    public interface IRepositorioMovimientos
    {
        Task Borrar(int id);
        Task InsertarMovimiento(Movimiento movimiento);
        Task<int> ObtenerIdMaximo();
        Task<IEnumerable<Movimiento>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina);
        Task<Movimiento> ObtenerPorId(int id);
        Task<int> ObtenerTotalMovimientos();
        Task Actualizar(int id, decimal monto);
        Task<decimal> getTotalEnPesos();
        Task<decimal> getTotalEnDolares();
        Task<IEnumerable<CuentaMontoDTO>> GetMontosPorCuenta(int idActivo);
        Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerIngresosPorClase(int year, int month);
        Task<IEnumerable<int>> ObtenerAniosMovimientos();
        Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerEgresosPorClase(int year, int month);
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerIngresosUltMeses();
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEgresosUltMeses();
        Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerIngresosPorClasePesos(int year, int month);
        Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerEgresosPorClasePesos(int year, int month);
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerIngresosUltMesesPesos();
        Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEgresosUltMesesPesos();
        Task<IEnumerable<bolsaEstadisticaViewModel>> ObtenerBolsaEstadistica(int idTipoActivo);
        Task<IEnumerable<bolsaGralEstadisticaViewModel>> ObtenerBolsaEstadisticaGral();
    }

    public class RepositorioMovimientos : IRepositorioMovimientos
    {
        private readonly string connectionString;

        public RepositorioMovimientos(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Movimiento>> ObtenerMovimientosPaginacion(int pagina, int cantidadPorPagina)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT  
	                        F.fecha Fecha, 
	                        MO.tipoMovimiento TipoMovimiento,
	                        CM.descripcion ClaseMovimientoNombre , 
	                        MO.comentario Comentario, 
	                        C.nombre CuentaNombre, 
	                        A.nombre ActivoNombre, 
	                        CAST(MO.monto AS decimal (18,2)) MONTO, 
	                        MO.IDMOVIMIENTO 
                        FROM [dbo].[Fact_Movimiento] MO 
                        INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = MO.idClaseMovimiento 
                        INNER JOIN Dim_Activo A ON A.idActivo = MO.idActivo 
                        INNER JOIN Dim_Cuenta C ON C.idCuenta = MO.idCuenta 
                        INNER JOIN Dim_Tiempo F ON MO.IDFECHA = F.IDFECHA 
                        ORDER BY MO.IDfecha DESC , idMovimiento DESC
                        OFFSET @Offset ROWS FETCH NEXT @CantidadPorPagina ROWS ONLY";

            return await connection.QueryAsync<Movimiento>(sql, new
            {
                Offset = (pagina - 1) * cantidadPorPagina,
                cantidadPorPagina = cantidadPorPagina
            });
            
        }

        public async Task<int> ObtenerTotalMovimientos()
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM Fact_Movimiento WHERE idClaseMovimiento IS NOT NULL";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> ObtenerIdMaximo()
        {
            using var conection = new SqlConnection(connectionString);

            var sql = "SELECT ISNULL(MAX(idMovimiento), 0) FROM Fact_Movimiento";
            return await conection.ExecuteScalarAsync<int>(sql);
        }

        public async Task InsertarMovimiento(Movimiento movimiento)
        {
            using var connection = new SqlConnection(connectionString);

            string sqlPrecioCotiz;

            if (movimiento.PrecioCotiz!= 0)
            {
                if (movimiento.ActivoNombre == "Peso Argentino")
                {
                    sqlPrecioCotiz = Convert.ToString(movimiento.PrecioCotiz, CultureInfo.InvariantCulture);
                }
                else
                {
                    sqlPrecioCotiz = Convert.ToString(1 / movimiento.PrecioCotiz, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                sqlPrecioCotiz = "(SELECT TOP 1 VALOR FROM [dbo].[Cotizacion_Activo] CA WHERE IDACTIVOCOMP = " +
                        "" + movimiento.IdActivo + " AND IDFECHA <= " + movimiento.Fecha.ToString("yyyyMMdd") + " " +
                        "ORDER BY idFecha DESC)";
            }

            string fecha = movimiento.Fecha.ToString("yyyyMMdd");

            var sql = "INSERT INTO Fact_Movimiento (IdMovimiento, idCuenta, idActivo, idFecha, tipoMovimiento, " +
                       " idClaseMovimiento, comentario, monto, precioCotiz) VALUES (@IdMovimiento, @IdCuenta, @IdActivo, " + fecha  + " , " + 
                       " @TipoMovimiento, @IdClaseMovimiento, @Comentario, @Monto, " + sqlPrecioCotiz + ")";

            await connection.ExecuteAsync(sql, movimiento);

        }
        
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Fact_Movimiento WHERE idMovimiento = @Id", new {id});

        }

        public async Task Actualizar(int id, decimal monto)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("UPDATE Fact_Movimiento SET monto = @Monto WHERE idMovimiento = @Id",
                    new { monto, id });
        }

        public async Task<Movimiento> ObtenerPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Movimiento>(@"SELECT  
	                                                                        FM.IdMovimiento IdMovimiento, 
	                                                                        C.IdCuenta IdCuenta,
	                                                                        C.nombre CuentaNombre,
	                                                                        A.idActivo IdActivo,
	                                                                        A.nombre ActivoNombre,
	                                                                        T.idFecha IdFecha,
	                                                                        T.fecha Fecha,
	                                                                        FM.tipoMovimiento TipoMovimiento,
	                                                                        FM.idClaseMovimiento IdClaseMovimiento,
	                                                                        CM.descripcion ClaseMovimientoNombre,
	                                                                        FM.comentario Comentario,
	                                                                        FM.Monto Monto, 
	                                                                        FM.precioCotiz PrecioCotiz
                                                                        FROM [dbo].[Fact_Movimiento] FM
                                                                        INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                                                                        INNER JOIN [dbo].[Dim_Cuenta] C ON C.idCuenta = FM.idCuenta
                                                                        INNER JOIN [dbo].[Dim_Tiempo] T ON T.idFecha = FM.idFecha
                                                                        LEFT JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = FM.idClaseMovimiento
                                                                        WHERE FM.idMovimiento = @Id
                                                                        ", new { id });
        }

        public async Task<decimal> getTotalEnPesos()
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT CAST( SUM( CASE WHEN A.esReferencia = 1 THEN MONTO ELSE CASE WHEN " +
                    "CA.valor IS NOT NULL THEN MONTO / CA.VALOR ELSE 0 END END) AS DECIMAL (18,2)) * " +
                    "(SELECT VALOR FROM [dbo].[Cotizacion_Activo] WHERE TIPO = 'BLUE' AND IDFECHA = (SELECT " +
                    "MAX(IDFECHA) FROM [dbo].[Cotizacion_Activo])) TOT FROM [dbo].[Fact_Movimiento] M INNER JOIN " +
                    "Dim_Activo A ON M.idActivo = A.idActivo LEFT JOIN [dbo].[Cotizacion_Activo] CA ON " +
                    "CA.idActivoComp = A.idActivo AND CA.idFecha = (SELECT MAX(IDFECHA) FROM " +
                    "[dbo].[Cotizacion_Activo]) AND CA.tipo <> 'TARJETA' AND CA.tipo <> 'BOLSA'";
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task<decimal> getTotalEnDolares()
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT CAST( SUM( CASE WHEN A.esReferencia = 1 THEN MONTO ELSE CASE WHEN " +
                    "CA.valor IS NOT NULL THEN MONTO / CA.VALOR ELSE 0 END END) AS DECIMAL (18,2)) tot FROM " +
                    "[dbo].[Fact_Movimiento] M INNER JOIN Dim_Activo A ON M.idActivo = A.idActivo LEFT JOIN " +
                    "[dbo].[Cotizacion_Activo] CA ON CA.idActivoComp = A.idActivo AND CA.idFecha = (SELECT " +
                    "MAX(IDFECHA) FROM [dbo].[Cotizacion_Activo]) AND CA.tipo <> 'TARJETA' AND CA.tipo <> " +
                    "'BOLSA'";
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task<IEnumerable<CuentaMontoDTO>> GetMontosPorCuenta(int idActivo)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = "SELECT C.nombre Cuenta, SUM(FM.MONTO)  Monto FROM " +
                        "[dbo].[Fact_Movimiento] FM INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo INNER " +
                        "JOIN [dbo].[Dim_Cuenta] C ON C.idCuenta = FM.idCuenta WHERE A.idActivo = @idActivo " +
                        "GROUP BY C.nombre HAVING SUM(FM.MONTO) > 0";

            return await connection.QueryAsync<CuentaMontoDTO>(sql, new { idActivo });
        }

        public async Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerIngresosPorClase(int year, int month)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        CM.descripcion ClaseMovimiento, ROUND(SUM(FM.monto / FM.precioCotiz), 2) Total
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN Dim_ClaseMovimiento CM ON FM.idClaseMovimiento = CM.idClaseMovimiento
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Ingreso' AND  T.anio = @Year AND T.mes = @Month
                        AND CM.descripcion <> 'Ajuste Saldos Ingreso'
                        GROUP BY CM.descripcion
                        ORDER BY SUM(FM.monto / FM.precioCotiz) DESC";

            return await connection.QueryAsync<MovimientoClaseTotalViewModel>(sql, new { year, month });
        }

        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerIngresosUltMeses()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"WITH ULTIMOSMESES AS(SELECT  TOP 6 
	                        T.anio,
	                        T.mes,T.mesNombre , ROUND(SUM(FM.monto / FM.precioCotiz), 2) Total
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN Dim_ClaseMovimiento CM ON FM.idClaseMovimiento = CM.idClaseMovimiento
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Ingreso' 
                        AND CM.descripcion <> 'Ajuste Saldos Ingreso'
                        GROUP BY T.ANIO, T.mes, T.mesNombre
                        ORDER BY T.ANIO, T.mes DESC)
                        SELECT * 
                        FROM ULTIMOSMESES
                        ORDER BY anio ASC, mes ASC;";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql);
        }

        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEgresosUltMeses()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"WITH ULTIMOSMESES AS(SELECT  TOP 6 
	                        T.anio,
	                        T.mes,T.mesNombre , - ROUND(SUM(FM.monto / FM.precioCotiz), 2) Total
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN Dim_ClaseMovimiento CM ON FM.idClaseMovimiento = CM.idClaseMovimiento
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Egreso' 
                        AND CM.descripcion <> 'Ajuste Saldos Egreso'
                        GROUP BY T.ANIO, T.mes, T.mesNombre
                        ORDER BY T.ANIO, T.mes DESC)
                        SELECT * 
                        FROM ULTIMOSMESES
                        ORDER BY anio ASC, mes ASC;";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql);
        }


        public async Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerEgresosPorClase(int year, int month)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        CM.descripcion ClaseMovimiento, ROUND(- SUM(FM.monto / FM.precioCotiz), 2) Total
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN Dim_ClaseMovimiento CM ON FM.idClaseMovimiento = CM.idClaseMovimiento
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Egreso' AND  T.anio = @Year AND T.mes = @Month
                        AND CM.descripcion <> 'Ajuste Saldos Egreso'
                        GROUP BY CM.descripcion
                        ORDER BY - SUM(FM.monto / FM.precioCotiz) DESC";

            return await connection.QueryAsync<MovimientoClaseTotalViewModel>(sql, new { year, month });
        }

        public async Task<IEnumerable<int>> ObtenerAniosMovimientos()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        DISTINCT T.anio 
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        ORDER BY T.anio DESC
                        ";

            return await connection.QueryAsync<int>(sql);
        }

        public async Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerIngresosPorClasePesos(int year, int month)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        CM.descripcion ClaseMovimiento
	                        , ROUND(SUM(TOTALPESOS), 2) Total
                        FROM
                        (
                        SELECT 
	                        FM.*
	                        ,
		                        CASE WHEN A.SIMBOLO = 'ARS' 
		                        THEN FM.MONTO 
		                        ELSE (FM.MONTO / FM.PRECIOCOTIZ) 
		                        * (SELECT VALOR FROM Cotizacion_Activo WHERE IDFECHA = 
			                        (SELECT MAX(IDFECHA) FROM Cotizacion_Activo
			                        ) 
		                        AND idActivoComp = 1 AND TIPO = 'BLUE')
		                         END TOTALPESOS
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Ingreso' AND  T.anio = @Year AND T.mes = @Month
                        ) T1
                        INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = T1.idClaseMovimiento
                        GROUP BY CM.descripcion
                        ORDER BY SUM(TOTALPESOS) DESC
                        ";

            return await connection.QueryAsync<MovimientoClaseTotalViewModel>(sql, new { year, month });
        }

        public async Task<IEnumerable<MovimientoClaseTotalViewModel>> ObtenerEgresosPorClasePesos(int year, int month)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        CM.descripcion ClaseMovimiento
	                        , ROUND(- SUM(TOTALPESOS), 2) Total
                        FROM
                        (
                        SELECT 
	                        FM.*
	                        ,
		                        CASE WHEN A.SIMBOLO = 'ARS' 
		                        THEN FM.MONTO 
		                        ELSE (FM.MONTO / FM.PRECIOCOTIZ) 
		                        * (SELECT VALOR FROM Cotizacion_Activo WHERE IDFECHA = 
			                        (SELECT MAX(IDFECHA) FROM Cotizacion_Activo
			                        ) 
		                        AND idActivoComp = 1 AND TIPO = 'BLUE')
		                         END TOTALPESOS
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                        INNER JOIN Dim_Tiempo T ON T.idFecha = FM.idFecha
                        WHERE FM.tipoMovimiento = 'Egreso' AND  T.anio = @Year AND T.mes = @Month
                        ) T1
                        INNER JOIN [dbo].[Dim_ClaseMovimiento] CM ON CM.idClaseMovimiento = T1.idClaseMovimiento
                        GROUP BY CM.descripcion
                        ORDER BY - SUM(TOTALPESOS) DESC
                        ";

            return await connection.QueryAsync<MovimientoClaseTotalViewModel>(sql, new { year, month });
        }

        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerIngresosUltMesesPesos()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"WITH ULTIMOSMESES AS (
                        SELECT TOP 6
	                        T.anio, T.mes, T.mesNombre, ROUND(SUM(S1.VALORPESOS), 2) Total
                        FROM
                        (
                        SELECT 
	                        FM.*, 
	                        CASE WHEN FM.idActivo = 2 THEN 1 ELSE CA.VALOR END COTIZACTUAL, 
	                        CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END VALORUSD, 
	                        (
		                        SELECT VALOR FROM Cotizacion_Activo WHERE IDFECHA = 
			                        (SELECT MAX(IDFECHA) FROM Cotizacion_Activo
			                        ) 
		                        AND idActivoComp = 1 AND TIPO = 'BLUE'
	                        ) * (CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END) VALORPESOS
                        FROM [dbo].[Fact_Movimiento] FM
                        LEFT JOIN [dbo].[Cotizacion_Activo] CA ON FM.idActivo = CA.idActivoComp AND 
	                        CA.idFecha = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) AND CA.tipo IN('NA', 'BLUE')
                        WHERE FM.tipoMovimiento = 'Ingreso' AND FM.idClaseMovimiento IS NOT NULL
                        ) S1
                        INNER JOIN Dim_Tiempo T ON S1.idFecha = T.idFecha
                        GROUP BY T.anio, T.mes, T.mesNombre
                        order by t.anio DESC, T.mes DESC)
                        SELECT * 
                        FROM ULTIMOSMESES
                        ORDER BY ANIO ASC, MES ASC;
                        ";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql);
        }
        public async Task<IEnumerable<MovimientoUlt6MesesViewModel>> ObtenerEgresosUltMesesPesos()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"WITH ULTIMOSMESES AS (
                        SELECT TOP 6
	                        T.anio, T.mes, T.mesNombre, ROUND(- SUM(S1.VALORPESOS), 2) Total
                        FROM
                        (
                        SELECT 
	                        FM.*, 
	                        CASE WHEN FM.idActivo = 2 THEN 1 ELSE CA.VALOR END COTIZACTUAL, 
	                        CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END VALORUSD, 
	                        (
		                        SELECT VALOR FROM Cotizacion_Activo WHERE IDFECHA = 
			                        (SELECT MAX(IDFECHA) FROM Cotizacion_Activo
			                        ) 
		                        AND idActivoComp = 1 AND TIPO = 'BLUE'
	                        ) * (CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END) VALORPESOS
                        FROM [dbo].[Fact_Movimiento] FM
                        LEFT JOIN [dbo].[Cotizacion_Activo] CA ON FM.idActivo = CA.idActivoComp AND 
	                        CA.idFecha = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) AND CA.tipo IN('NA', 'BLUE')
                        WHERE FM.tipoMovimiento = 'Egreso' AND FM.idClaseMovimiento IS NOT NULL
                        ) S1
                        INNER JOIN Dim_Tiempo T ON S1.idFecha = T.idFecha
                        GROUP BY T.anio, T.mes, T.mesNombre
                        order by t.anio DESC, T.mes DESC)
                        SELECT * 
                        FROM ULTIMOSMESES
                        ORDER BY ANIO ASC, MES ASC;
                        ";

            return await connection.QueryAsync<MovimientoUlt6MesesViewModel>(sql);
        }

        public async Task<IEnumerable<bolsaEstadisticaViewModel>> ObtenerBolsaEstadistica(int idTipoActivo)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                    A.nombre NombreActivo
	                    , A.simbolo Simbolo
                        , SUM(FM.monto) Cantidad
	                    , ROUND(SUM(FM.MONTO / FM.PRECIOCOTIZ),2) ValorOrigen
	                    ,ROUND(SUM(CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END), 2) ValorActual
                    FROM [dbo].[Fact_Movimiento] FM
                    INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                    LEFT JOIN [dbo].[Cotizacion_Activo] CA ON FM.idActivo = CA.idActivoComp AND 
	                    CA.idFecha = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) AND CA.tipo IN('NA', 'BLUE')
                    WHERE A.idTipoActivo = @IdTipoActivo
                    GROUP BY A.nombre, A.simbolo
                    ORDER BY SUM(CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END) DESC";

            return await connection.QueryAsync<bolsaEstadisticaViewModel>(sql, new { idTipoActivo });
        }
        public async Task<IEnumerable<bolsaGralEstadisticaViewModel>> ObtenerBolsaEstadisticaGral()
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                        TA.nombre TipoActivo
	                        , ROUND(SUM(FM.MONTO / FM.PRECIOCOTIZ),2) ValorOrigen
	                        ,ROUND(SUM(CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END), 2) ValorActual
                        FROM [dbo].[Fact_Movimiento] FM
                        INNER JOIN [dbo].[Dim_Activo] A ON A.idActivo = FM.idActivo
                        INNER JOIN [dbo].[Dim_Tipo_Activo] TA ON TA.idTipoActivo = A.idTipoActivo
                        LEFT JOIN [dbo].[Cotizacion_Activo] CA ON FM.idActivo = CA.idActivoComp AND 
	                        CA.idFecha = (SELECT MAX(IDFECHA) FROM Cotizacion_Activo) AND CA.tipo IN('NA', 'BLUE')
                        WHERE TA.entorno = 'Bolsa'
                        GROUP BY TA.nombre
                        ORDER BY SUM(CASE WHEN FM.idActivo = 2 THEN FM.MONTO ELSE FM.MONTO/CA.valor END) DESC";

            return await connection.QueryAsync<bolsaGralEstadisticaViewModel>(sql);
        }
    }
}
