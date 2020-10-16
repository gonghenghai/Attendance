using Attendance;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace attendance01
{
    class Program
    {
        static void Main(string[] args)
        {
            Methods methods = new Methods();
            //methods.ImportDataToMySQL();
            methods.AnalysisResultsToMySQL();
        }
    }
}
