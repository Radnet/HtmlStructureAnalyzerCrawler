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

namespace WebWorker
{
    public class Worker
    {
        // MongoDB Helpers
        private static MongoDBWrapper mongoDB = new MongoDBWrapper();
        static void Main(string[] args)
        {
            Console.WriteLine("Worker Started");
            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            LogWriter.Info("Worker Started");

            // Configuring MongoDB Wrapper
            string fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PSW, Consts.MONGO_AUTH_DB,
                fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_PROCESSED_URLS_COLLECTION);

            //Parser
            PageParser parser = new PageParser();

            // Creating Instance of Web Requests Server
            WebRequests server = new WebRequests();

            try
            {
                Console.WriteLine();
                Console.WriteLine("##################################");
                Console.WriteLine();
                Console.WriteLine("Get html to parse from SQS...");

                FullPage fullPageObj;
                string actualReceiptHandle;
                
                while(GetHTMLFromSQSQueue(out fullPageObj, out actualReceiptHandle))
                {
                    Console.WriteLine("Parse html of \"" + fullPageObj.Url + "\"");
                    // Parsing Page Tags
                    PageInfo parsedPage = parser.ParsePageStats(fullPageObj);

                    Console.WriteLine("Add parsed info to DB...");
                    // Save parsed page on stats DB
                    mongoDB.AddToStats(parsedPage);

                    // Delete Message from SQS
                    Console.WriteLine("Deleting the message from SQS.");
                    DeleteSQSMessage(actualReceiptHandle);
                }
                Console.WriteLine();
                Console.WriteLine("##################################");
                Console.WriteLine();
                Console.WriteLine("No more messages on SQS finish.");
            }
            catch (Exception ex)
            {
                LogWriter.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPageObj"></param>
        /// <returns></returns>
        private static bool GetHTMLFromSQSQueue(out FullPage fullPageObj, out string actualReceiptHandle)
        {
            // Receive Page from SQS
            // Preparing SQS 
            // SQS uses N.Virginia as default
            AmazonSQSClient amazonSQSClient = new AmazonSQSClient(Consts.USER_ACCESS_KEY_ID, Consts.USER_SECRET_ACCESS_KEY, Amazon.RegionEndpoint.USEast1);

            //Receiving a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = Consts.SQS_QUEUE_URL;
            receiveMessageRequest.MaxNumberOfMessages = 1; // MAX OF 1 MESSAGE A TIME
            ReceiveMessageResponse receiveMessageResponse = amazonSQSClient.ReceiveMessage(receiveMessageRequest);

            // Verify if an message was received
            if (receiveMessageResponse.Messages.Count > 0)
            {
                // Deserializing message
                fullPageObj = JsonConvert.DeserializeObject<FullPage>(receiveMessageResponse.Messages.First().Body);
                actualReceiptHandle = receiveMessageResponse.Messages.First().ReceiptHandle;
                return true;
            }
            // No message found on Queue
            else
            {
                fullPageObj = null;
                actualReceiptHandle = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualReceiptHandle"></param>
        private static void DeleteSQSMessage(string actualReceiptHandle)
        {
            // Deleting a message from SQS
            // Preparing SQS 
            // SQS uses N.Virginia as default
            AmazonSQSClient amazonSQSClient = new AmazonSQSClient(Consts.USER_ACCESS_KEY_ID, Consts.USER_SECRET_ACCESS_KEY, Amazon.RegionEndpoint.USEast1);

            DeleteMessageRequest deleteRequest = new DeleteMessageRequest();
            deleteRequest.QueueUrl = Consts.SQS_QUEUE_URL;
            deleteRequest.ReceiptHandle = actualReceiptHandle;
            amazonSQSClient.DeleteMessage(deleteRequest);
        }
    }
}
