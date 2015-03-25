using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class QueuedPage
    {
        public ObjectId _id  { get; set; }
        public string Domain { get; set; }
        public string Url    { get; set; }
        public bool   IsBusy { get; set; }
        public int    Tries  { get; set; }
    }
}
