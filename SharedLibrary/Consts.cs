using System;
using System.Collections.Generic;
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
        public static readonly string MONGO_SERVER           = "ec2-54-85-5-69.compute-1.amazonaws.com";
        public static readonly string MONGO_PORT             = "27017";
        public static readonly string MONGO_USER             = "sysdba";
        public static readonly string MONGO_PASS             = "crawlerdbadministrator";
        public static readonly string MONGO_DATABASE         = "UrlsControl";
        public static readonly string MONGO_COLLECTION       = "ProcessedUrls";
        public static readonly string MONGO_STATS_COLLECTION = "Stats";
        public static readonly string QUEUED_URLS_COLLECTION = "UrlQueue";
        public static readonly string MONGO_AUTH_DB          = "admin";
        public static readonly int    MONGO_TIMEOUT          = 10000;

        // AWS
        //User props
        public static readonly string USER_NAME              = "HtmlQueue";
        public static readonly string USER_ACCESS_KEY_ID     = "AKIAJ353T6VIOM3EWJNA";
        public static readonly string USER_SECRET_ACCESS_KEY = "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";
        //QUEUE props
        public static readonly string SQS_QUEUE_URL = "https://sqs.us-east-1.amazonaws.com/461546948012/HtmlQueue";
    }
}
