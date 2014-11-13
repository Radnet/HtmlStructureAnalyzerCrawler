using BDC.BDCCommons;
using SharedLibrary;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilsLib;

namespace WebWorker
{
    public class Worker
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

            try
            {  
                FullPage fullPageObj;
                
                while(getFullPageFromQueue(out fullPageObj))
                {
                    // Parsing Page Tags
                    PageInfo parsedPage = parser.ParsePageStats(fullPageObj);

                    // SAVE ON DB

                }
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);
            }
        }

        private static bool getFullPageFromQueue(out FullPage fullPageObj)
        {
            throw new NotImplementedException();
        }
    }
}
