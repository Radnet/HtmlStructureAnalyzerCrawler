using BDC.BDCCommons;
using SharedLibrary;
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

            // Retry Counter (Used for exponential wait increasing logic)
            int retryCounter = 0;

            try
            {
                // Get Page URL 
                string pageUrl  = "http://g1.globo.com/index.html";
                
                // Parsing domain
                int startIndex  = pageUrl.IndexOf("/") + 2;
                int endIndex    = pageUrl.IndexOf("/", startIndex);
                string domain   = pageUrl.Substring(startIndex, endIndex - startIndex);

                // Configuring server and Issuing Request
                server.EncodingDetection = WebRequests.CharsetDetection.DefaultCharset;
                string page = server.Get(pageUrl);

                // Sanity Check
                if (String.IsNullOrEmpty(page) || server.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // Renewing WebRequest Object to get rid of Cookies
                    server = new WebRequests();

                    // Inc. retry counter
                    retryCounter++;

                    Console.WriteLine("Retrying:" + retryCounter);

                    // Checking for maximum retry count
                    double waitTime;
                    if (retryCounter >= 11)
                    {
                        waitTime = TimeSpan.FromMinutes(35).TotalMilliseconds;

                        // Removing Page from the database (this page may have expired)
                        //mongoDB.RemoveFromQueue(appUrl);
                    }
                    else
                    {
                        // Calculating next wait time ( 2 ^ retryCounter seconds)
                        waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryCounter)).TotalMilliseconds;
                    }

                    // Hiccup to avoid Domain blocking connections in case of heavy traffic from the same IP
                    Thread.Sleep(Convert.ToInt32(waitTime));
                }
                // Sanity Check OK!
                else
                {
                    // Reseting retry counter
                    retryCounter = 0;

                    // Parsing PAge Tags
                    PageInfo parsedApp = parser.ParsePageStats(page, pageUrl, domain);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);
            }
        }
    }
}
