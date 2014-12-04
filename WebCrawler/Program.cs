using Amazon.SQS;
using Amazon.SQS.Model;
using SharedLibrary;
using SharedLibrary.MongoDB;
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
        // MongoDB Helpers
        private static MongoDBWrapper mongoDB = new MongoDBWrapper();
        static void Main(string[] args)
        {
            // Configuring MongoDB Wrapper
            string fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, 
                fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

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
            // Get Queued page that is not on "busy" stats AND mark as busy
            pageToParse = mongoDB.FindAndModify();
            if(pageToParse != null)
            {
                return true;
            }
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
                    insetHtmlOnSQSQueue(page);

                    //Parser Internal urls
                    PageParser parser = new PageParser();
                    List<string> internalUrl = parser.GetInternalLinks(page, pageToParse.Domain);

                    //Insert Internal urls in Queue to be processed
                    foreach (string internalLink in internalUrl)
                    {
                        InsertPageOnURLQueue(internalLink, pageToParse.Domain);
                    }

                    //Remove page from Queue and insert on Processed collection
                    ChangePageStatusToProcessed(pageToParse);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        private static void insetHtmlOnSQSQueue(string page)
        {
            // Insert Page on SQS
            // Preparing SQS 
            // SQS uses N.Virginia as default
            AmazonSQSClient amazonSQSClient = new AmazonSQSClient(Consts.USER_ACCESS_KEY_ID,Consts.USER_SECRET_ACCESS_KEY,Amazon.RegionEndpoint.USEast1);

            //Sending a message
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = Consts.SQS_QUEUE_URL; //URL from initial queue creation
            sendMessageRequest.MessageBody = page;
            amazonSQSClient.SendMessage(sendMessageRequest);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private static bool IsProcessedPage(QueuedPage page)
        {
            // Verify if page was processed, if TRUE, REMOVE from QUEUE
            if(mongoDB.IsPageProcessed(page))
            {
                mongoDB.RemoveFromQueue(page);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool InsertPageOnURLQueue(string url, string domain)
        {
            QueuedPage newPage = new QueuedPage();
            newPage.Url = url;
            newPage.Domain = domain;
            newPage.IsBusy = false;

            // Insert url on Queue
            mongoDB.AddToQueue(newPage);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private static bool ChangePageStatusToProcessed(QueuedPage page)
        {
            // Remove page from Queue
            mongoDB.RemoveFromQueue(page);

            //Insert on Processed
            mongoDB.AddToProcessed(page);
            return false;
        }
    }
}
