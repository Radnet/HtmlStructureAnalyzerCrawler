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
            CountInternalAndExternalLinks(page.Url);

            // Domain and URL
            InfoResults.Url = page.Url;
            InfoResults.Domain = page.Domain;

            return InfoResults;
        }

        /// <summary>
        /// Count all internal and external links of the page
        /// </summary>
        /// <returns></returns>
        private void CountInternalAndExternalLinks(string originalUrl)
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
                if ( IsInternal(node.GetAttributeValue("href", " "), originalUrl) )
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
        /// Get all intenal links on a page 
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
                    
                    // If it's an internal link
                    if(IsInternal(link, originalUrl))
                    {
                        RemoveAnchor(link, out link);
                        internalLinks.Add(getAbsoluteUrl(link, originalUrl));
                    }
                }
            }

            return internalLinks;
        }

        /// <summary>
        /// Removes the Anchor of a link
        /// </summary>
        /// <param name="link"></param>
        private bool RemoveAnchor(string link, out string linkOut)
        {
            if(link.Contains('#'))
            {
                // Anchor composition of a link is like "href = http://www.xpto.com/xyz#anchor1"
                linkOut = link.Split('#')[0];
                return true;
            }
            else
            {
                linkOut = link;
                return false;
            }
        }

        /// <summary>
        ///  Determines if a url is internal or not 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsInternal(string link, string originalUrl)
        {
            Uri uriResult;
            // Verify link integrity
            if (Uri.TryCreate(link, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
            {
                // Set Uris
                Uri linkUri = new Uri(link, UriKind.RelativeOrAbsolute);
                Uri originalUri = new Uri(originalUrl, UriKind.RelativeOrAbsolute);

                // Make it absolute if it's relative
                if (!linkUri.IsAbsoluteUri)
                {
                    linkUri = new Uri(originalUri, linkUri);
                }

                // If it's an internal link
                if (linkUri.IsWellFormedOriginalString() && originalUri.IsBaseOf(linkUri))
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

        /// <summary>
        /// returns the absolute url from a relative url
        /// </summary>
        /// <param name="link"></param>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public string getAbsoluteUrl(string link, string originalUrl)
        {
            // Set Uris
            Uri linkUri = new Uri(link, UriKind.RelativeOrAbsolute);
            Uri originalUri = new Uri(originalUrl, UriKind.RelativeOrAbsolute);
            
            // Make it absolute if it's relative
            if (!linkUri.IsAbsoluteUri)
            {
                linkUri = new Uri(originalUri, linkUri);
            }

            return linkUri.AbsoluteUri;
        }
    }
}
