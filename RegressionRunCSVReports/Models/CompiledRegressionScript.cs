namespace RegressionRunCSVReports.Models
{
    public class CompiledRegressionScript
    {
        public string Test { get; set; }
        public string LastRunDate { get; set; }
        public string LastSuccessfulRunDate { get; set; }
        public double TotalRegressionRuns { get; set; }
        public double SuccessfulRegressionRuns { get; set; }
        public string SuccessPercentage { get; set; }
    }
}