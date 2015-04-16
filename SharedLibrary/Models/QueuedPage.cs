using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class QueuedPage : CoreQueuedPage
    {
        public ObjectId _id { get; set; }
        public int Tries    { get; set; }
    }
    public class Bootstrapper : CoreQueuedPage
    {
        public ObjectId _id { get; set; }
        public bool UsedFlag  { get; set; }
    }
    public class CoreQueuedPage
    {
        public string Domain { get; set; }
        public string Url    { get; set; }
        public bool   IsBusy { get; set; }
    }
}
