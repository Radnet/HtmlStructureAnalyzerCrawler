using NLog;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using Classificator.SimpleHelpers;
using WebUtilsLib;
using System.Collections.Generic;
using SharedLibrary;
using SharedLibrary.Models;

namespace Classificator
{
	class Program
	{
		public static FlexibleOptions ProgramOptions { get; private set; }

		/// <summary>
		/// Main program entry point.
		/// </summary>
		static void Main (string[] args)
		{
			// set error exit code
			System.Environment.ExitCode = -50;
			try
			{
				// load configurations
				ProgramOptions = ConsoleUtils.Initialize (args, true);           
					
				// start execution
				Execute (ProgramOptions);

				// check before ending for waitForKeyBeforeExit option
				if (ProgramOptions.Get ("waitForKeyBeforeExit", false))
					ConsoleUtils.WaitForAnyKey ();
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger ().Fatal (ex);

				// check before ending for waitForKeyBeforeExit option
				if (ProgramOptions.Get ("waitForKeyBeforeExit", false))
					ConsoleUtils.WaitForAnyKey ();

				ConsoleUtils.CloseApplication (-60, true);
			}
			// set success exit code
			ConsoleUtils.CloseApplication (0, false);
		}
		
		static Logger logger = LogManager.GetCurrentClassLogger ();
		static DateTime started = DateTime.UtcNow;

		private static void Execute (FlexibleOptions options)
		{
			logger.Info ("Start");

            // Page counter
            int pageCounter = 0;

            // Get Domain
            string domain = ConsoleUtils.ProgramOptions.Get ("DOMAIN");
		   
			// Create empty list of urls to be processed
            List<string> urlList = new List<string> ();
            urlList.Add(ConsoleUtils.ProgramOptions.Get ("START_PAGE"));
			
			// Get number of pagess
			int pageCountLimit = ConsoleUtils.ProgramOptions.Get ("PROCESSED_PAGE_LIMIT", 0);
			
            // Creating Instance of Web Requests Server
            WebRequests server = new WebRequests();

            //Server Response
            string html;

            // Stats List
            List<PageInfo> pageInfoList = new List<PageInfo> ();

            // Crawl X(pageCountLimit) web pages of the domain
            while (urlList.Count > 0 && pageCounter < pageCountLimit)
            {
                Console.WriteLine("Geting HTML...");
                
                // Get Url
                string url = PopRandom(urlList);

                // Get Page
                html = server.Get(url);

                // Sanity Check
                if (String.IsNullOrEmpty(html) || server.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (server.StatusCode == System.Net.HttpStatusCode.NotFound || (int)server.StatusCode == 0)
                    {
                        Console.WriteLine("Page " + url + " not Found! Go to next page");
                        continue;
                    }
                }
                else
                {
                    // Increase counter
                    pageCounter++;

                    //Parser Internal urls for Queue feeding 
                    Console.WriteLine("Getting internal links...");

                    PageParser parser = new PageParser();
                    List<string> internalLinksList = parser.GetInternalLinks(html, domain, url);

                    Console.WriteLine("(" + internalLinksList.Count + ") internal links found.");

                    //Insert Internal urls in List to be processed if List has less than 10.000 urls
                    if(urlList.Count < 10000)
                    {
                        urlList.AddRange (internalLinksList);
                    }

                    pageInfoList.Add ( parser.ParsePageStats(new FullPage { Domain = domain, Url = url, Html = html }) );
                }
            }

            //######## Classify #########
            // Initialize counters
            double textAndStylers = 0;
            double interaction    = 0;
            double visual         = 0;
            double form           = 0;
            double table          = 0;
            double organizers     = 0;
            double links          = 0;

            // Calculate percentages
            foreach(PageInfo pgInfo in pageInfoList)
            {
                // Text And Stylers
                textAndStylers = textAndStylers           +
                                 pgInfo.TagsCount.title   +
                                 pgInfo.TagsCount.header  +
                                 pgInfo.TagsCount.h1      +
                                 pgInfo.TagsCount.h2      +
                                 pgInfo.TagsCount.h3      +
                                 pgInfo.TagsCount.h4      +
                                 pgInfo.TagsCount.h5      +
                                 pgInfo.TagsCount.h6      +
                                 pgInfo.TagsCount.p       +
                                 pgInfo.TagsCount.br      +
                                 pgInfo.TagsCount.footer  +
                                 pgInfo.TagsCount.article +
                                 pgInfo.TagsCount.aside   +
                                 pgInfo.TagsCount.font    +
                                 pgInfo.TagsCount.em      +
                                 pgInfo.TagsCount.strong  +
                                 pgInfo.TagsCount.i       +
                                 pgInfo.TagsCount.u       +
                                 pgInfo.TagsCount.ins     +
                                 pgInfo.TagsCount.del     +
                                 pgInfo.TagsCount.mark    +
                                 pgInfo.TagsCount.dfn     +
                                 pgInfo.TagsCount.code    +
                                 pgInfo.TagsCount.samp    +
                                 pgInfo.TagsCount.kbd     +
                                 pgInfo.TagsCount.var     +
                                 pgInfo.TagsCount.pre     +
                                 pgInfo.TagsCount.q       +
                                 pgInfo.TagsCount.small   +
                                 pgInfo.TagsCount.sub     +
                                 pgInfo.TagsCount.sup     +
                                 pgInfo.TagsCount.summary +
                                 pgInfo.TagsCount.time    ;

                // Interaction
                interaction =    interaction               +
                                 pgInfo.TagsCount.audio    +
                                 pgInfo.TagsCount.track    +
                                 pgInfo.TagsCount.button   +
                                 pgInfo.TagsCount.input    +
                                 pgInfo.TagsCount.datalist +
                                 pgInfo.TagsCount.dl       +
                                 pgInfo.TagsCount.dt       +
                                 pgInfo.TagsCount.dd       +
                                 pgInfo.TagsCount.option   +
                                 pgInfo.TagsCount.keygen   +
                                 pgInfo.TagsCount.output   ;

                // Visual
                visual =        visual                      +
                                pgInfo.TagsCount.img        +
                                pgInfo.TagsCount.figure     +
                                pgInfo.TagsCount.figcaption +
                                pgInfo.TagsCount.canvas     +
                                pgInfo.TagsCount.map        +
                                pgInfo.TagsCount.meter      +
                                pgInfo.TagsCount.progress   +
                                pgInfo.TagsCount.source     ;

                // Form
                form =          form                      +
                                pgInfo.TagsCount.form     +
                                pgInfo.TagsCount.fieldset +
                                pgInfo.TagsCount.legend   +
                                pgInfo.TagsCount.li       +
                                pgInfo.TagsCount.ol       +
                                pgInfo.TagsCount.ul       +
                                pgInfo.TagsCount.menu     +
                                pgInfo.TagsCount.menuitem ;

                // Table
                table =         table                     +
                                pgInfo.TagsCount.table    +
                                pgInfo.TagsCount.td       +
                                pgInfo.TagsCount.th       +
                                pgInfo.TagsCount.tfoot    +
                                pgInfo.TagsCount.col      +
                                pgInfo.TagsCount.colgroup ;

                // Organizers
                organizers =    organizers                +
                                pgInfo.TagsCount.head     +
                                pgInfo.TagsCount.div      +
                                pgInfo.TagsCount.frameset +
                                pgInfo.TagsCount.frame    +
                                pgInfo.TagsCount.link     +
                                pgInfo.TagsCount.nav      ;


                // Links
                links =         links                   +
                                pgInfo.TagsCount.link   ;
            }

            // Calculate total
            double totalTagsCount = textAndStylers + interaction + visual + form + table + organizers + links;

            // Calculate percentage of each class
            double perc_textAndStylers = (textAndStylers / totalTagsCount) * 100;
            double perc_interaction    = (interaction    / totalTagsCount) * 100;
            double perc_visual         = (visual         / totalTagsCount) * 100;
            double perc_form           = (form           / totalTagsCount) * 100;
            double perc_table          = (table          / totalTagsCount) * 100;
            double perc_organizers     = (organizers     / totalTagsCount) * 100;
            double perc_links          = (links          / totalTagsCount) * 100;

            // Get percentages for each class
            // News
            double news_textAndStylers = ConsoleUtils.ProgramOptions.Get ("news_textAndStylers" , 0.0);
            double news_interaction    = ConsoleUtils.ProgramOptions.Get ("news_interaction"    , 0.0);
            double news_visual         = ConsoleUtils.ProgramOptions.Get ("news_visual"         , 0.0);
            double news_form           = ConsoleUtils.ProgramOptions.Get ("news_form"           , 0.0);
            double news_table          = ConsoleUtils.ProgramOptions.Get ("news_table"          , 0.0);
            double news_organizers     = ConsoleUtils.ProgramOptions.Get ("news_organizers"     , 0.0);
            double news_links          = ConsoleUtils.ProgramOptions.Get ("news_links"          , 0.0);
                                                                                                   
            // Eccomerce                                                                                
            double ecos_textAndStylers = ConsoleUtils.ProgramOptions.Get ("ecos_textAndStylers" , 0.0);
            double ecos_interaction    = ConsoleUtils.ProgramOptions.Get ("ecos_interaction"    , 0.0);
            double ecos_visual         = ConsoleUtils.ProgramOptions.Get ("ecos_visual"         , 0.0);
            double ecos_form           = ConsoleUtils.ProgramOptions.Get ("ecos_form"           , 0.0);
            double ecos_table          = ConsoleUtils.ProgramOptions.Get ("ecos_table"          , 0.0);
            double ecos_organizers     = ConsoleUtils.ProgramOptions.Get ("ecos_organizers"     , 0.0);
            double ecos_links          = ConsoleUtils.ProgramOptions.Get ("ecos_links"          , 0.0);
                                                                                                   
            // Governament                                                                         
            double govs_textAndStylers = ConsoleUtils.ProgramOptions.Get ("govs_textAndStylers" , 0.0);
            double govs_interaction    = ConsoleUtils.ProgramOptions.Get ("govs_interaction"    , 0.0);
            double govs_visual         = ConsoleUtils.ProgramOptions.Get ("govs_visual"         , 0.0);
            double govs_form           = ConsoleUtils.ProgramOptions.Get ("govs_form"           , 0.0);
            double govs_table          = ConsoleUtils.ProgramOptions.Get ("govs_table"          , 0.0);
            double govs_organizers     = ConsoleUtils.ProgramOptions.Get ("govs_organizers"     , 0.0);
            double govs_links          = ConsoleUtils.ProgramOptions.Get ("govs_links"          , 0.0);

            // Initialize distances
            double news_distance = 0;
            double ecos_distance = 0;
            double govs_distance = 0;

            // For each class calculate distance for 1
            // News
            news_distance = news_distance + (1 / Math.Abs(news_textAndStylers - perc_textAndStylers ));
            news_distance = news_distance + (1 / Math.Abs(news_interaction    - perc_interaction    ));
            news_distance = news_distance + (1 / Math.Abs(news_visual         - perc_visual         ));
            news_distance = news_distance + (1 / Math.Abs(news_form           - perc_form           ));
            news_distance = news_distance + (1 / Math.Abs(news_table          - perc_table          ));
            news_distance = news_distance + (1 / Math.Abs(news_organizers     - perc_organizers     ));
            news_distance = news_distance + (1 / Math.Abs(news_links          - perc_links          ));

            // Eccomerce 
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_textAndStylers - perc_textAndStylers ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_interaction    - perc_interaction    ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_visual         - perc_visual         ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_form           - perc_form           ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_table          - perc_table          ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_organizers     - perc_organizers     ));
            ecos_distance = ecos_distance + (1 / Math.Abs(ecos_links          - perc_links          ));

            // Governament   
            govs_distance = govs_distance + (1 / Math.Abs(govs_textAndStylers - perc_textAndStylers ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_interaction    - perc_interaction    ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_visual         - perc_visual         ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_form           - perc_form           ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_table          - perc_table          ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_organizers     - perc_organizers     ));
            govs_distance = govs_distance + (1 / Math.Abs(govs_links          - perc_links          ));

            // Show points
            Console.WriteLine("news_distance = " + news_distance);
            Console.WriteLine("ecos_distance = " + ecos_distance);
            Console.WriteLine("govs_distance = " + govs_distance);

            // Classify by the smallest distance
            if(news_distance < ecos_distance)
            {
                // Its a NEWS
                if(news_distance < govs_distance)
                {
                    Console.WriteLine("It's a NEWS !");
                }
                // Its a GOV
                else if (govs_distance < news_distance)
                {
                    Console.WriteLine("It's a GOVERNAMENTAL !");
                }
                else
                {
                    Console.WriteLine("It's a TIE between NEWS and GOVERNAMENTAL !");
                }
            }
            else if (ecos_distance < news_distance)
            {
                // Its a ECO
                if (ecos_distance < govs_distance)
                {
                    Console.WriteLine("It's a ECCOMERCE !");
                }
                // Its a GOV
                else if (govs_distance < ecos_distance)
                {
                    Console.WriteLine("It's a GOVERNAMENTAL !");
                }
                else
                {
                    Console.WriteLine("It's a TIE between ECCOMERCE and GOVERNAMENTAL !");
                }
            }
            // Its a tie beetween NEWS and ECO
            else
            {
                Console.WriteLine("It's a TIE between NEWS and ECCOMERCE !");
            }
            
            Console.ReadKey();

            //###########################

			logger.Info ("End");
		}

        public static string PopRandom (List<string> list)
        {
            Random rnd = new Random();

            int r = rnd.Next(list.Count);

            string popedItem = (string)list[r];

            list.RemoveAt(r);

            return popedItem;
        }
	}
}