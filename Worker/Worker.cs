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
            // Configuring Log Object Threshold
            LogWriter.Threshold = TLogEventLevel.Information;
            LogWriter.Info("Worker Started");

            // Configuring MongoDB Wrapper
            string fullServerAddress = String.Join(":", Consts.MONGO_SERVER, Consts.MONGO_PORT);
            mongoDB.ConfigureDatabase(Consts.MONGO_USER, Consts.MONGO_PASS, Consts.MONGO_AUTH_DB,
                fullServerAddress, Consts.MONGO_TIMEOUT, Consts.MONGO_DATABASE, Consts.MONGO_COLLECTION);

            //Parser
            PageParser parser = new PageParser();

            // Creating Instance of Web Requests Server
            WebRequests server = new WebRequests();

            try
            {  
                FullPage fullPageObj;
                
                while(getHTMLFromSQSQueue(out fullPageObj))
                {
                    // Parsing Page Tags
                    PageInfo parsedPage = parser.ParsePageStats(fullPageObj);

                    // Save parsed page on stats DB
                    mongoDB.AddToStats(parsedPage);
                }
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
        private static bool getHTMLFromSQSQueue(out FullPage fullPageObj)
        {
            // Receive Page from SQS
            // Preparing SQS 
            // SQS uses N.Virginia as default
            AmazonSQSClient amazonSQSClient = new AmazonSQSClient(Consts.USER_ACCESS_KEY_ID, Consts.USER_SECRET_ACCESS_KEY, Amazon.RegionEndpoint.USEast1);

            //Receiving a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = Consts.SQS_QUEUE_URL;
            receiveMessageRequest.MaxNumberOfMessages = 1;
            ReceiveMessageResponse receiveMessageResponse = amazonSQSClient.ReceiveMessage(receiveMessageRequest);

            // Verify if an message was received
            if (receiveMessageResponse.Messages.Count > 0)
            {
                // Deserializing message
                fullPageObj = JsonConvert.DeserializeObject<FullPage>(receiveMessageResponse.Messages.First().ToString());
                return true;
            }
            // No message found on Queue
            else
            {
                fullPageObj = null;
                return false;
            }
        }
    }
}
