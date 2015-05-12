using SharedLibrary.SimpleHelpers;
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
       // public static readonly string TAG_PREFIX = "//";

        // Page limit
        public static readonly int    PROCESSED_PAGE_LIMIT            = ConsoleUtils.ProgramOptions.Get("PROCESSED_PAGE_LIMIT", 0);

        // MongoDB - Remote Server
        public static readonly string MONGO_SERVER                    = ConsoleUtils.ProgramOptions.Get ("MONGO_SERVER");
        public static readonly string MONGO_PORT                      = ConsoleUtils.ProgramOptions.Get ("MONGO_PORT");
        public static readonly string MONGO_USER                      = ConsoleUtils.ProgramOptions.Get ("MONGO_USER");
        public static readonly string MONGO_PSW                       = ConsoleUtils.ProgramOptions.Get ("MONGO_PSW");
        public static readonly string MONGO_DATABASE                  = ConsoleUtils.ProgramOptions.Get ("MONGO_DATABASE");
        public static readonly string MONGO_PROCESSED_URLS_COLLECTION = ConsoleUtils.ProgramOptions.Get ("MONGO_PROCESSED_URLS_COLLECTION");
        public static readonly string MONGO_STATS_COLLECTION          = ConsoleUtils.ProgramOptions.Get ("MONGO_STATS_COLLECTION");
        public static readonly string MONGO_QUEUED_URLS_COLLECTION    = ConsoleUtils.ProgramOptions.Get ("MONGO_QUEUED_URLS_COLLECTION");
        public static readonly string MONGO_BOOTSTRAPPER_COLLECTION   = ConsoleUtils.ProgramOptions.Get ("MONGO_BOOTSTRAPPER_COLLECTION");
        public static readonly string MONGO_HTML_STORAGE_COLLECTION   = ConsoleUtils.ProgramOptions.Get ("MONGO_HTML_STORAGE_COLLECTION");
        public static readonly string MONGO_AUTH_DB                   = ConsoleUtils.ProgramOptions.Get ("MONGO_AUTH_DB");
        public static readonly int    MONGO_TIMEOUT                   = ConsoleUtils.ProgramOptions.Get ("MONGO_TIMEOUT", 0);

        // AWS
        //User props
        public static readonly string USER_NAME                       = ConsoleUtils.ProgramOptions.Get ("USER_NAME");
        public static readonly string USER_ACCESS_KEY_ID              = ConsoleUtils.ProgramOptions.Get ("USER_ACCESS_KEY_ID");
        public static readonly string USER_SECRET_ACCESS_KEY          = ConsoleUtils.ProgramOptions.Get ("USER_SECRET_ACCESS_KEY");
        
        //QUEUE props
        public static readonly string SQS_QUEUE_URL = "https://sqs.us-east-1.amazonaws.com/461546948012/HtmlQueue";
    }
}
