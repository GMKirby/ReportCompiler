using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TriageReports.Models;

namespace TriageReports.Logic
{
    class TriageReport
    {
        public static async Task Run()
        {
            var env = string.Empty;

            Console.Write("Enter your env:");
            env = "WOR"; //Console.ReadLine().ToUpper();

            var dateForFile = DateTime.Now.ToString("yyyy-MM-dd");

            var path = KnownFolders.Downloads.Path;
            var directory = string.Format(@"{0}\Regression Run JSONs\{1}", path, env);
            var filePath = @$"{directory}\cucumber.18592.json";

            var fileText = File.ReadAllText(filePath).Trim();

            var features = JsonConvert.DeserializeObject<List<Feature>>(fileText);

            foreach(var feature in features)
            {
                foreach(var script in feature.Elements)
                {
                    var temp = JsonConvert.DeserializeObject<List<ZefaTest>>((string)script);
                    
                    var test = "123";

                }
            }


            Console.ReadKey();
        }
    }
}
