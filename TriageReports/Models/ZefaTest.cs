using System.Collections.Generic;

namespace TriageReports.Models
{
    public class ZefaTest
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<Step> Steps { get; set; }
    }
}
