using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PageInfo
    {
        public TagQTD   TagsCount;
        public int      ExternalLinksCount;
        public int      InternalLinksCount;

        public PageInfo()
        {
            TagsCount = new TagQTD();
            
            // Zero for all tags count
            PropertyInfo[] TagQTDProperties = typeof(TagQTD).GetProperties();
            foreach(PropertyInfo property in TagQTDProperties)
            {
                property.SetValue(TagsCount, 0);
            }

            // Zero for external links
            ExternalLinksCount = 0;

            // Zero for internal links
            InternalLinksCount = 0;
        }
    }
}
