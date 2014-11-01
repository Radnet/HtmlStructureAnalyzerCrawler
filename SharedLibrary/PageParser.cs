using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PageParser
    {
        public  PageInfo InfoResults;
        private HtmlDocument Map;

        public PageInfo ParsePage(string page, string pageUrl, string domain)
        {
            // Loading Html Document with Play Store content
            Map = new HtmlDocument();
            Map.LoadHtml(page);

            InfoResults = new PageInfo();

            // Counting
            CountAllTags();
            CountInternalAndExternalLinks(pageUrl,domain);

            return InfoResults;
        }

        /// <summary>
        /// Count all internal and external links of the page
        /// </summary>
        /// <returns></returns>
        private void CountInternalAndExternalLinks(string pageUrl, string domain)
        {
            // Zeroing
            InfoResults.InternalLinksCount = 0;
            InfoResults.ExternalLinksCount = 0;

            // Checking for nodes of Internal Links
            HtmlNodeCollection nodes = Map.DocumentNode.SelectNodes("//a/@href");
            
            // Checking consistency
            if (nodes == null)
                return;

            // Counting Internal and External Links
            foreach(var node in nodes)
            {
                // If hef url has the domain, it's an internal link
                if (node.GetAttributeValue("href", " ").Contains(domain))
                    InfoResults.InternalLinksCount++;
                else
                    InfoResults.ExternalLinksCount++;
            }
        }

        /// <summary>
        /// Count all tags listed on TagQTD Class
        /// </summary>
        private void CountAllTags()
        {
            // For each tag defined onTagQTD Class, count it on page
            PropertyInfo[] TagQTDProperties = typeof(TagQTD).GetProperties();
            foreach(PropertyInfo property in TagQTDProperties)
            {
                property.SetValue(InfoResults.TagsCount, CountSingleTag(property.Name));
            }
        }

        /// <summary>
        /// Parse web page HTML by the parameter tag and count results
        /// </summary>
        /// <param name="response">HTML of the page</param>
        /// <returns>Count of tag occurrence it finds</returns>
        private int CountSingleTag(string tag)
        {
            // Checking for nodes of tag
            HtmlNodeCollection nodes = Map.DocumentNode.SelectNodes("//"+tag);

            // Checking consistency
            if (nodes == null)
                return 0;

            return nodes.Count;
        }
    }
}
