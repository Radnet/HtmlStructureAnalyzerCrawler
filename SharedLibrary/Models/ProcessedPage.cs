using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class ProcessedPage
    {
        public ObjectId _id { get; set; }
        public string Domain { get; set; }
        public string Url    { get; set; }
    }
}
