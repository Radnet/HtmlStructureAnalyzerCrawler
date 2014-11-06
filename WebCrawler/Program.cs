using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtilsLib;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get Domains
            string domain = "http://g1.globo.com";

            // Crawl while there are domains in Queue
            //while(getDomains(out domain))
            //{
                CrawlUrls(domain);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        private static void CrawlUrls(string domain)
        {
            // Creating Instance of Web Requests Server
            using(WebRequests server = new WebRequests())
            {
                // Get Page
                string page = server.Get(domain);

                //Parser
                PageParser parser = new PageParser();
                List<string> internalLinks = parser.GetInternalLinks(page, domain);

                //Insert Internal Links in Queue
            }
        }
    }
}
