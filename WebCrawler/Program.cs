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
            // Crawl while there are domains in Queue
            QueuedPage pageToParse;

            while(GetQueuedPage(out pageToParse))
            {
                CrawlUrls(pageToParse);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageToParse"></param>
        /// <returns></returns>
        private static bool GetQueuedPage(out QueuedPage pageToParse)
        {
            pageToParse = new QueuedPage();
            // Get Queued page that is not on "busy" stats AND mark as busy

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        private static void CrawlUrls(QueuedPage pageToParse)
        {
            // Check if page has alredy been processed
            if (!IsProcessedPage(pageToParse))
            {
                // Creating Instance of Web Requests Server
                using (WebRequests server = new WebRequests())
                {
                    // Get Page
                    string page = server.Get(pageToParse.Url);
                    
                    // Put page html on SQS Queue
                    insetHtmlOnQueue(page);

                    //Parser Internal urls
                    PageParser parser = new PageParser();
                    List<string> internalUrl = parser.GetInternalLinks(page, pageToParse.Domain);

                    //Insert Internal urls in Queue to be processed
                    foreach (string internalLink in internalUrl)
                    {
                        InsertUrlOnQueue(internalLink);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        private static void insetHtmlOnQueue(string page)
        {
            // Insert Page on SQS
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageToParse"></param>
        /// <returns></returns>
        private static bool IsProcessedPage(QueuedPage pageToParse)
        {
            // Verify if page was processed, if TRUE, REMOVE from QUEUE
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool InsertUrlOnQueue(string url)
        {
            // Put url on Queue to be processed
            return false;
        }
    }
}
