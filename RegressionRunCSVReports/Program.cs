﻿using System;
using System.Threading.Tasks;
using RegressionRunCSVReports.Logic;

namespace RegressionRunCSVReports
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ReportCompiler.Run();
        }
    }
}