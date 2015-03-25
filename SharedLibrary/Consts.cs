using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class Consts
    {
        // XPaths
        public static readonly string TAG_PREFIX = "//";

        // MongoDB - Remote Server
        public static readonly string MONGO_SERVER                    = ConfigurationManager.AppSettings.Get("MONGO_SERVER");
        public static readonly string MONGO_PORT                      = ConfigurationManager.AppSettings.Get("MONGO_PORT");
        public static readonly string MONGO_USER                      = ConfigurationManager.AppSettings.Get("MONGO_USER");
        public static readonly string MONGO_PSW                       = ConfigurationManager.AppSettings.Get("MONGO_PSW");
        public static readonly string MONGO_DATABASE                  = ConfigurationManager.AppSettings.Get("MONGO_DATABASE");
        public static readonly string MONGO_PROCESSED_URLS_COLLECTION = ConfigurationManager.AppSettings.Get("MONGO_PROCESSED_URLS_COLLECTION");
        public static readonly string MONGO_STATS_COLLECTION          = ConfigurationManager.AppSettings.Get("MONGO_STATS_COLLECTION");
        public static readonly string MONGO_QUEUED_URLS_COLLECTION    = ConfigurationManager.AppSettings.Get("MONGO_QUEUED_URLS_COLLECTION");
        public static readonly string MONGO_HTML_STORAGE_COLLECTION   = ConfigurationManager.AppSettings.Get("MONGO_HTML_STORAGE_COLLECTION");
        public static readonly string MONGO_AUTH_DB                   = ConfigurationManager.AppSettings.Get("MONGO_AUTH_DB");
        public static readonly int    MONGO_TIMEOUT                   = Int32.Parse(ConfigurationManager.AppSettings.Get("MONGO_TIMEOUT"));

        // AWS
        //User props
        public static readonly string USER_NAME                       = ConfigurationManager.AppSettings.Get("USER_NAME");
        public static readonly string USER_ACCESS_KEY_ID              = ConfigurationManager.AppSettings.Get("USER_ACCESS_KEY_ID");
        public static readonly string USER_SECRET_ACCESS_KEY          = ConfigurationManager.AppSettings.Get("USER_SECRET_ACCESS_KEY");
        //QUEUE props
        public static readonly string SQS_QUEUE_URL = "https://sqs.us-east-1.amazonaws.com/461546948012/HtmlQueue";
    }
}
