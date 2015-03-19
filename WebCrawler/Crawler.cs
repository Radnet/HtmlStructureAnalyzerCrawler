using Amazon.SQS;
using Amazon.SQS.Model;
using BDC.BDCCommons;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebUtilsLib;

namespace WebCrawler
{
    class Crawler
    {
        // MongoDB Helpers
        private static MongoDBWrapper mongoDB = new MongoDBWrapper();

        static void Main(string[] args)
        {
            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            LogWriter.Info("Worker Started");

            Console.WriteLine("Open MongoDB connection...");

            // Configuring MongoDB Wrapper
            string fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB, fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            // Retry Counter (Used for exponential wait increasing logic)
            int retryCounter = 0;

            // Crawl while there are urls in Queue
            QueuedPage pageToParse;

            while(true)
            {
                Console.WriteLine("Get page to parse.");
                if (GetNonBusyQueuedPage(out pageToParse))
                {
                    Console.WriteLine("Crawl : " + pageToParse.Url);
                    CrawlUrls(pageToParse);
                    // Hiccup to avoid domain blocking connections in case of heavy traffic from the same IP
                    Thread.Sleep(Convert.ToInt32(TimeSpan.FromSeconds(15).TotalMilliseconds));
                }
                else
                {
                    // Inc. retry counter
                    retryCounter++;

                    Console.WriteLine("Get from Queue did not succeeded. Retry Number " + retryCounter);

                    double waitTime;
                    // Checking for maximum retry count
                    if (retryCounter > 20)
                    {
                        Console.WriteLine("Process reched maximum retries.... Probably there is no more pages to process.");
                        Console.WriteLine("BYE BYE");
                        break;
                    }
                    // Checking for biggest acepted retry count
                    else if (retryCounter >= 11)
                    {
                        waitTime = TimeSpan.FromMinutes(35).TotalMilliseconds;
                    }
                    else
                    {
                        // Calculating next wait time ( 2 ^ retryCounter seconds)
                        waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryCounter)).TotalMilliseconds;
                    }

                    Console.WriteLine("Sleep a little bit...");
                    // Hiccup to wait for new
                    Thread.Sleep(Convert.ToInt32(waitTime));
                }
            }
            Console.WriteLine("End of process...");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageToParse"></param>
        /// <returns></returns>
        private static bool GetNonBusyQueuedPage(out QueuedPage pageToParse)
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
            // Retry Counter (Used for exponential wait increasing logic)
            int retryCounter = 0;

            // Check if page has alredy been processed
            if (!IsProcessedPage(pageToParse))
            {
                // Creating Instance of Web Requests Server
                WebRequests server = new WebRequests();

                //Server Response
                string html;

                do // while (String.IsNullOrEmpty(html) || server.StatusCode != System.Net.HttpStatusCode.OK);
                {
                    Console.WriteLine("Geting HTML...");
                    // Get Page
                    html = server.Get(pageToParse.Url);

                    // Sanity Check
                    if (String.IsNullOrEmpty(html) || server.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        LogWriter.Info("Error opening page : " + pageToParse.Url);

                        // Inc. retry counter
                        retryCounter++;

                        Console.WriteLine("Retrying:" + retryCounter);

                        // Checking for maximum retry count waitTime
                        double waitTime = 0;
                        if (retryCounter >= 11)
                        {
                            waitTime = TimeSpan.FromMinutes(35).TotalMilliseconds;
                        }
                        // Cheking for problematic page
                        else if (retryCounter >= 13)
                        {
                            // Probably my IP is blocked for this domain or the page is expired...

                            // Ckeck if maximmum of crawlers that tryed this page and did not succeeded to was reached.
                            if(mongoDB.PageRetries(pageToParse) >= 3)
                            {
                                Console.WriteLine("This page is probably unreachable. Remove it from DB.");
                                // Removing Page from the database (this the page may have expired)
                                mongoDB.RemoveFromQueue(pageToParse);
                                return;
                            }
                            // Give up and let other crawler try.
                            else 
                            {
                                Console.WriteLine("Probably IP is blocked. Give up and let other crawler try.");
                                return;
                            }
                        }
                        else
                        {
                            // Calculating next wait time ( 2 ^ retryCounter seconds)
                            waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryCounter)).TotalMilliseconds;
                            
                            // Maybe this IP is blocked by the url Domain. Flag url as not busy 
                            // so that other crawlers can try to process it
                            if (retryCounter >= 7)
                            {
                                mongoDB.ToggleBusyPage(pageToParse, false);
                                mongoDB.IncrisePageTriesCounter(pageToParse);
                            }
                        }

                        // Hiccup to avoid blocking connections in case of heavy traffic from the same IP
                        Console.WriteLine("Hiccup to avoid blocking connections. WaitTime = " + waitTime);
                        Thread.Sleep(Convert.ToInt32(waitTime));
                    }
                    else
                    {
                        retryCounter = 0;

                        // Put page html on SQS Queue
                        Console.WriteLine("Sending HTML to SQS...");
                        insetHtmlOnSQSQueue(pageToParse, html);

                        // Save page on DB for future access if needed

                        //Parser Internal urls
                        PageParser parser = new PageParser();
                        List<string> internalUrl = parser.GetInternalLinks(html, pageToParse.Domain, pageToParse.Url);

                        //Insert Internal urls in Queue to be processed
                        foreach (string internalLink in internalUrl)
                        {
                            // Verify if url is NOT alredy on Queue or processed
                            if (!mongoDB.IspageOnQueue(internalLink) && !mongoDB.IsPageProcessed(internalLink))
                            {
                                InsertPageOnURLQueue(internalLink, pageToParse.Domain);
                            }

                        }

                        //Remove page from Queue and insert on Processed collection
                        ChangePageStatusToProcessed(pageToParse);
                    }
                }
                while (String.IsNullOrEmpty(html) || server.StatusCode != System.Net.HttpStatusCode.OK); 
            }
            else
            {
                Console.WriteLine("Page Already Processed.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        private static void insetHtmlOnSQSQueue(QueuedPage page, string html)
        {
            // Insert Page on SQS
            // Preparing SQS 
            // SQS uses N.Virginia as default
            AmazonSQSClient amazonSQSClient = new AmazonSQSClient(Consts.USER_ACCESS_KEY_ID,Consts.USER_SECRET_ACCESS_KEY,Amazon.RegionEndpoint.USEast1);

            // Prepare message model
            FullPage pageToSQS = new FullPage();
            pageToSQS.Domain   = page.Domain;
            pageToSQS.Url      = page.Url;
            pageToSQS.Html     = html;

            //Preparing message
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl           = Consts.SQS_QUEUE_URL; //URL from initial queue creation
            sendMessageRequest.MessageBody        = JsonConvert.SerializeObject(pageToSQS);
            
            //send
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
            return mongoDB.AddToQueue(newPage);
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
