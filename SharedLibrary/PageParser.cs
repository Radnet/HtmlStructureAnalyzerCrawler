using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PageParser
    {
        /// <summary>
        /// Parse web page HTML by the parameter tag and count results
        /// </summary>
        /// <param name="response">HTML of the page</param>
        /// <returns>Count of tag occurrence it finds</returns>
        public int CountTag(string html, string tag)
        {
            // Loading Html Document with Play Store content
            HtmlDocument map = new HtmlDocument();
            map.LoadHtml(html);

            // Checking for nodes of tag
            HtmlNodeCollection nodes = map.DocumentNode.SelectNodes("//"+tag);

            // Checking consistency
            if (nodes == null)
                return 0;

            return nodes.Count;
        }
    }
}
