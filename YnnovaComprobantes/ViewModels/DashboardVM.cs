namespace YnnovaComprobantes.ViewModels
{
    public class DashboardVM
    {
        // KPIs (Tarjetas Superiores)
        public decimal GastoTotal { get; set; }
        public int Pendientes { get; set; }
        public int Aprobados { get; set; }
        public int Observados { get; set; }

        // Datos para los Gráficos ECharts
        public ChartDataVM EvolucionGastos { get; set; }
        public List<PieDataVM> DistribucionDinero { get; set; }
        public ChartDataVM GastosPorConcepto { get; set; }
        public ChartDataVM EstadoLiquidaciones { get; set; }
    }

    public class ChartDataVM
    {
        public List<string> Labels { get; set; }
        public List<decimal> Values { get; set; }
    }

    public class PieDataVM
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
