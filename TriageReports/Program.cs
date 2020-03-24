using System;
using System.Threading.Tasks;
using TriageReports.Logic;

namespace TriageReports
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await TriageReport.Run();
        }
    }
}
