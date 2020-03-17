using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text;
using System.Threading.Tasks;

namespace RegressionRunCSVReports
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var env = string.Empty;

            Console.Write("Enter your env:");
            env = Console.ReadLine().ToUpper();

            var dateForFile = DateTime.Now.ToString("yyyy-MM-dd");

            var allScripts = new List<RegressionScript>();
            var datedScripts = new List<DatedRegressionScript>();
            var directory = string.Format(@"{0}\Regression Run CSVs\{1}", KnownFolders.Downloads.Path, env);
            var compiledRegressionReportFileOutputLocation = string.Format(@"{0}\{1} {2} Compiled Regression Report.csv", KnownFolders.Downloads.Path, env, dateForFile);
            var scriptsByDateRegressionReportFileOutputLocation = string.Format(@"{0}\{1} {2} Scripts By Date Regression Report.csv", KnownFolders.Downloads.Path, env, dateForFile);
            var regressionRunDates = new List<string>();

            foreach (var fileName in Directory.GetFiles(directory).OrderBy(item => item))
            {
                var fileNameSplit = fileName.Split(@"\").ToList();
                var date = fileNameSplit.LastOrDefault().Split('.').FirstOrDefault();
                var records = GetRecordsFromCSV(fileName);

                allScripts.AddRange(records);
                datedScripts.AddRange(DateScripts(records, date));
                regressionRunDates.Add(date);
            }

            WriteRecordsToCSV(compiledRegressionReportFileOutputLocation, CreateCompiledReport(datedScripts));
            Console.WriteLine(string.Format("Compiled Regression Report Complete. See Report at {0}{1}", Environment.NewLine, compiledRegressionReportFileOutputLocation));

            Console.WriteLine("");

            await CreateScriptsByDateReport(datedScripts, regressionRunDates.OrderBy(item => item).ToList(), scriptsByDateRegressionReportFileOutputLocation);
            Console.WriteLine(string.Format("Scripts By Date Regression Report Complete. See Report at {0}{1}", Environment.NewLine, scriptsByDateRegressionReportFileOutputLocation));

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        private static async Task CreateScriptsByDateReport(List<DatedRegressionScript> scripts, List<string> dates, string filePath)
        {
            var csvHeaders = new StringBuilder();
            var scriptsStringBuilder = new StringBuilder();
            var datePercentageStringBuilder = new StringBuilder();

            csvHeaders.Append("Script, ");
            datePercentageStringBuilder.Append("Success Percentage, ");

            foreach (var date in dates)
            {
                csvHeaders.Append(string.Format("{0}, ", date));
            }

            foreach (var script in scripts.DistinctBy(item => item.Test))
            {
                scriptsStringBuilder.Append(script.Test + ", ");

                foreach (var date in dates)
                {
                    var datedScript = scripts.FirstOrDefault(item => item.Test == script.Test && item.Date == date);
                    if (datedScript != null)
                    {
                        var status = datedScript.Status == "PASS" ? "Successful" : "Failed";
                        scriptsStringBuilder.Append(status + ", ");
                    }
                    else
                    {
                        scriptsStringBuilder.Append(", ");
                    }
                }

                scriptsStringBuilder.Append(Environment.NewLine);
            }

            foreach (var date in dates)
            {
                var totalScriptsOnDateCount = (double)scripts.Count(item => item.Date == date);
                var succesfulScriptsOnDateCount = (double)scripts.Count(item => item.Date == date && item.Status == "PASS");

                datePercentageStringBuilder.Append((succesfulScriptsOnDateCount / totalScriptsOnDateCount * 100).ToString("0.00") + ", ");
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();

            }

            await File.WriteAllTextAsync(filePath, string.Format("{0}{1}{2}{3}", csvHeaders, Environment.NewLine, scriptsStringBuilder, datePercentageStringBuilder));
        }

        private static List<CompiledRegressionScript> CreateCompiledReport(List<DatedRegressionScript> allScripts)
        {
            var result = new List<CompiledRegressionScript>();

            foreach (var script in allScripts.DistinctBy(item => item.Test))
            {
                var scripts = allScripts.Where(item => item.Test == script.Test);
                var succesfulScripts = scripts.Where(item => item.Status == "PASS");

                var totalScriptsCount = (double)scripts.Count();
                var successfulScriptsCount = (double)succesfulScripts.Count();

                var lastRun = scripts.OrderByDescending(item => item.Date).FirstOrDefault();
                var lastSuccessfulRun = succesfulScripts.OrderByDescending(item => item.Date).FirstOrDefault();

                result.Add(new CompiledRegressionScript
                {
                    Test = string.Format(@"https://jira.rosiplus.de/browse/{0}", script.Test),
                    SuccessfulRegressionRuns = successfulScriptsCount,
                    TotalRegressionRuns = totalScriptsCount,
                    SuccessPercentage = (successfulScriptsCount / totalScriptsCount * 100).ToString("0.00"),
                    LastRunDate = lastRun?.Date,
                    LastSuccessfulRunDate = lastSuccessfulRun?.Date
                });
            }

            return result;
        }

        private static List<DatedRegressionScript> DateScripts(List<RegressionScript> scripts, string date)
        {
            var result = new List<DatedRegressionScript>();

            foreach (var script in scripts)
            {
                result.Add(new DatedRegressionScript
                {
                    Date = date,
                    Test = string.Format(@"https://jira.rosiplus.de/browse/{0}", script.Test),
                    Status = script.Status
                });
            }

            return result;
        }

        private static List<RegressionScript> GetRecordsFromCSV(string filePath)
        {
            using StreamReader reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader);
            return csv.GetRecords<RegressionScript>().ToList();
        }

        private static void WriteRecordsToCSV(string filePath, List<CompiledRegressionScript> records)
        {
            using StreamWriter writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer);
            csv.WriteRecords(records);
        }
    }

    public class RegressionScript
    {
        public string Test { get; set; }
        public string Status { get; set; }
    }

    public class DatedRegressionScript : RegressionScript
    {
        public string Date { get; set; }
    }

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