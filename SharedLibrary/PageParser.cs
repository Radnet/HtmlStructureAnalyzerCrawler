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
        public List<string> GetInternalLinks(string page, string domain)
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
                    if (IsInternal(link, domain, out link))
                    {
                        internalLinks.Add(link);
                    }
                }
            }

            return internalLinks;
        }

        /// <summary>
        /// Verify if an link is part of a domain
        /// </summary>
        /// <param name="url"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsInternal(string url, string domain)
        {
            // If hef url has the domain, it's an internal link
            if (url.Contains(domain))
                return true;
            else
            {
                // Try to get an internal link that does not contais and diret reference to domain
                int indexOfFirtBar = url.IndexOf("/");
                if (indexOfFirtBar > -1)
                {
                    string sufix = url.Substring(0, indexOfFirtBar + 1);
                    // If the first part of the link(before '/') does NOT contains an.(dot) 
                    // it means that this link is an reference for a page of the same domain 
                    if (!sufix.Contains('.') && !sufix.Contains("http") && !sufix.Contains("https"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        ///  Verify if an link is part of a domain and treat it if necessary
        /// </summary>
        /// <param name="url"></param>
        /// <param name="domain"></param>
        /// <param name="TreatedUrl">Treated Url</param>
        /// <returns></returns>
        public bool IsInternal(string url, string domain, out string TreatedUrl)
        {
            // If hef url has the domain, it's an internal link
            if (url.Contains(domain))
            {
                TreatedUrl = url;
                return true;
            }
            else if (IsInternal(url, domain))
            {
                TreatedUrl = domain + "/" + url;
                return true;
            }
            else
            {
                TreatedUrl = url;
                return false;
            }
        }
    }
}
