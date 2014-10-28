using BDC.BDCCommons;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilsLib;

namespace WebWorker
{
    class Worker
    {
        static void Main(string[] args)
        {
            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            LogWriter.Info("Worker Started");

            //Parser
            PageParser parser = new PageParser();

            // Creating Instance of Web Requests Server
            WebRequests server = new WebRequests();



        }
    }
}
