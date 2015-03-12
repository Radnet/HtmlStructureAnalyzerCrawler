using HtmlAgilityPack;
using SharedLibrary.Models;
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

        public PageInfo ParsePageStats(FullPage page)
        {
            // Loading Html Document with content
            Map = new HtmlDocument();
            Map.LoadHtml(page.Html);

            InfoResults = new PageInfo();

            // Counting
            CountAllTags();
            CountInternalAndExternalLinks(page.Domain);

            // Domain and URL
            InfoResults.Url = page.Url;
            InfoResults.Domain = page.Domain;

            return InfoResults;
        }

        /// <summary>
        /// Count all internal and external links of the page
        /// </summary>
        /// <returns></returns>
        private void CountInternalAndExternalLinks(string domain)
        {
            // Zeroing
            InfoResults.InternalLinksCount = 0;
            InfoResults.ExternalLinksCount = 0;

            // Get nodes of Links
            HtmlNodeCollection nodes = Map.DocumentNode.SelectNodes("//a/@href");
            
            // Checking consistency
            if (nodes == null)
                return;

            // Counting Internal and External Links
            foreach(var node in nodes)
            {
                // If is an internal link
                if ( IsInternal(node.GetAttributeValue("href", " "), domain) )
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<string> GetInternalLinks(string page, string domain, string originalUrl)
        {
            // return obj
            List<string> internalLinks = new List<string>();
            
            // Loading Html Document with content
            Map = new HtmlDocument();
            Map.LoadHtml(page);

            // Parse
            // Checking for nodes of Internal Links
            HtmlNodeCollection nodes = Map.DocumentNode.SelectNodes("//a/@href");

            // Check if nodes is NOT empty
            if (!(nodes == null || nodes.Count == 0))
            {
                // Counting Internal and External Links
                foreach (var node in nodes)
                {
                    string link = node.GetAttributeValue("href", " ");
                    // If hef url has the domain, it's an internal link
                    if (IsInternalAndTreat(link, domain, originalUrl, out link))
                    {
                        internalLinks.Add(link);
                    }
                }
            }

            return internalLinks;
        }

        /// <summary>
        /// Verify if an link is internal and treat it if necessary
        /// </summary>
        /// <param name="url"></param>
        /// <param name="domain"></param>
        /// <param name="treatedUrl"> string that contains the treated internal url of the domain </param>
        /// <returns></returns>
        public bool IsInternalAndTreat(string url, string domain, string originalUrl, out string treatedUrl)
        {
            // If hef url has the domain, it's an internal link
            if (url.Contains(domain))
            {
                treatedUrl = url;
                return true;
            }
            // Try to get an internal link that does not contais and diret reference to domain
            else 
            {
                int indexOfFirstBar = url.IndexOf("/");
                if (indexOfFirstBar > -1)
                {
                    string sufix = url.Substring(0, indexOfFirstBar + 1);

                    // If url contains http or https, its an external link
                    if (sufix.Contains("http") || sufix.Contains("https"))
                    {
                        treatedUrl = url;
                        return false;
                    }
                    // it means that this link is an reference for a higher level page befor the original url
                    // ex: href = "../xpto.hmtl"
                    if (sufix.StartsWith(".."))
                    {
                        string uperLevel = originalUrl.Substring(0 , originalUrl.LastIndexOf("/"));
                        if(uperLevel.Contains("/"))
                        {
                            uperLevel = uperLevel.Substring(0, originalUrl.LastIndexOf("/"));
                        }
                        treatedUrl = uperLevel + url.Remove(0,2); // url remove 2 dots
                        return true;
                    }
                    // If there is nothing before the "/" it's a reference for a page in the domain root  
                    // ex.: href = "/xpto.hmtl"
                    else if (sufix.Length == 1)
                    {
                        treatedUrl = domain + url;
                        return true;
                    }
                    else
                    {
                        treatedUrl = originalUrl.Substring(0, originalUrl.LastIndexOf("/") + 1) + url;
                        return true;
                    }
                }
                else
                {
                    // It's an anchor
                    if (url[0] == '#')
                    {
                        treatedUrl = url;
                        return false;
                    }
                    else
                    {
                        treatedUrl = originalUrl.Substring(0, originalUrl.LastIndexOf("/") + 1) + url;
                        return true;
                    }

                }
            }
        }
    }
}
