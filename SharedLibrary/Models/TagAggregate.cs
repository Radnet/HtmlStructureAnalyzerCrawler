using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class TagAggregate
    {
        public IdId _id      { get; set; }
        public int sumResult { get; set; }
    }

    public class IdId
    {
        public string Domain { get; set; }
    }
}
